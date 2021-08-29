using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    public class MonoBehaviourPack_PatrolCenter : MonoBehaviourPack
    {
        const string HEADER_PATH_SELECTION = "Path Selection";
        const string HEADER_BEHAVIOUR = "Behaviour Settings";


        [Tooltip("If set up character will try to stay in patrol point. Otherwise will just walk randomly regardless of position")]
        public MonoPatrolCenter patrolCenter;
        bool isPatrolCenterSetUp => patrolCenter;

        [Space]
        [ShowIf("isPatrolCenterSetUp")]


        [BoxGroup(HEADER_BEHAVIOUR), Tooltip("? - for now just leave it at 0.85f or soo. Mixes random movement with center. Wip param will probably change")]
        [Range(0, 1)] public float centerMixFactor = 0.85f;

        [BoxGroup(HEADER_BEHAVIOUR), Tooltip("Ai movement point will be at distance in this range. Also takes into considerations path settings")]
        public RangedFloat walkDistance = new RangedFloat(3,7);

        [BoxGroup(HEADER_BEHAVIOUR), Tooltip("Distance to the target point at which we can stop movement. Warning if value is too low movement could never finish")]
        public float closeDistance = 1.0f;

        [Space]
        [BoxGroup(HEADER_BEHAVIOUR), Tooltip("how often the behaviour is selected. Actual chance depends on utility value of other behaviours")]
        public float utility = 1;

        [BoxGroup(HEADER_BEHAVIOUR), Tooltip("Behaviour execution time range, actual time will be randomized each behaviour usage")]
        public RangedFloat executionTime = new RangedFloat(1, 3);

        [BoxGroup(HEADER_BEHAVIOUR), Tooltip("Whole behaviour cooldown")]
        public Timer tCooldown;

        private new void Start()
        {
            base.Start();

            var monoAttentionSelector = controller.GetComponentInChildren<MonoAttentionSelector>();
            var inputHolder = controller.GetComponentInParent<InputHolder>();
            var seeker = controller.GetComponentInChildren<Pathfinding.Seeker>();

            AttentionMode modeIdle = monoAttentionSelector.idleMode;

            // states
            var statePatrol = modeIdle.AddNewState("Walk Randomly");

            // helpers
            MoveToDestinationNavigation moveToDestination = new MoveToDestinationNavigation(seeker);
            Timer tExecution = new Timer();

            statePatrol
                .SetUtility(utility)
                .AddCanEnter(tCooldown.IsReady)
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnBegin(() =>
                {
                    tExecution.RestartRandom(executionTime);
                    Vector3 destination = transform.position + (Random.insideUnitCircle * walkDistance.GetRandom()).To3D();

                    if (patrolCenter)
                    {
                        destination = Vector3.Lerp(patrolCenter.transform.position, destination, centerMixFactor);
                    }

                    moveToDestination.SetDestination(patrolCenter.Clamp(destination));
                })
                .AddOnUpdate(() =>
                {
                    inputHolder.positionInput = moveToDestination.ToDestination(closeDistance, 0.75f);
                })
                .AddOnEnd(() =>
                {
                    // restart cooldowns
                    tCooldown.Restart();
                })
                .AddShallReturn(tExecution.IsReady)
                .AddShallReturn(() => moveToDestination.IsCloseToDestination(closeDistance))
            ;
        }
    }
}
