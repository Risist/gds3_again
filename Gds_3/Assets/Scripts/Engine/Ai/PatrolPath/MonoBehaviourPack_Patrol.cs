using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    public class MonoBehaviourPack_Patrol : MonoBehaviourPack
    {
        const string HEADER_PATH_SELECTION = "Path Selection";
        const string HEADER_BEHAVIOUR = "Behaviour Settings";

        [Serializable]
        public class PatrolPathSettings
        {
            public PatrolPathFollower.ETraversalDirection traversalDirection;
            public MonoPatrolPath patrolPath;

            [Tooltip("Specific path selection cooldown")]
            public Timer tCooldown;
        }

        public PatrolPathSettings[] patrolPaths;


        [BoxGroup(HEADER_PATH_SELECTION), Tooltip("Maximal distance path is allowed for selection")]
        public float maxDistanceToPath;

        [BoxGroup(HEADER_BEHAVIOUR), Tooltip("Distance next node on the path will be selected")]
        public float nodeProximityDistance = 0.75f;

        [BoxGroup(HEADER_BEHAVIOUR), Tooltip("Behaviour execution time range, actual time will be randomized each behaviour usage")]
        public RangedFloat executionTime = new RangedFloat(1, 3);

        [BoxGroup(HEADER_BEHAVIOUR), Tooltip("Whole behaviour cooldown")]
        public Timer tCooldown;

        [BoxGroup(HEADER_BEHAVIOUR), Tooltip("how often the behaviour is selected. Actual chance depends on utility value of other behaviours")]
        public float utility = 1.0f;

        PatrolPathSettings currentPatrolPathSettings;
        int startingPathPointId;


        void SelectPatrolPath()
        {
            currentPatrolPathSettings = null;
            float minDistSqToPath = float.MaxValue;

            foreach (var it in patrolPaths)
            {
                if (!it.tCooldown.IsReady())
                    continue;
                
                if (!it.patrolPath.CanBeTraversed)
                    continue;

                var path = it.patrolPath.GetPatrolPath();
                int closestPathPointId = path.GetClosestPointId(transform.position);
                Vector3 closestPathPoint = path.pointList[closestPathPointId];
                float distSq = (transform.position - closestPathPoint).sqrMagnitude;

                if (distSq > maxDistanceToPath.Sq())
                    continue;

                if(distSq < minDistSqToPath)
                {
                    minDistSqToPath = distSq;
                    currentPatrolPathSettings = it;
                    startingPathPointId = closestPathPointId;
                }
            }
        }

        bool IsAnyPathAvailable()
        {
            foreach (var it in patrolPaths)
            {
                if (!it.tCooldown.IsReady())
                    continue;

                if (!it.patrolPath.CanBeTraversed)
                    continue;

                var path = it.patrolPath.GetPatrolPath();
                int closestPathPointId = path.GetClosestPointId(transform.position);
                Vector3 closestPathPoint = path.pointList[closestPathPointId];
                float distSq = (transform.position - closestPathPoint).sqrMagnitude;

                if (distSq > maxDistanceToPath.Sq())
                    continue;

                return true;
            }
            return false;
        }

        private new void Start()
        {
            base.Start();
            var monoAttentionSelector = controller.GetComponentInChildren<MonoAttentionSelector>();
            var inputHolder = controller.GetComponentInParent<InputHolder>();
            var seeker = controller.GetComponentInChildren<Pathfinding.Seeker>();

            AttentionMode modeIdle = monoAttentionSelector.idleMode;

            // states
            var statePatrol = modeIdle.AddNewState("Patrol");

            // helpers
            PatrolPathFollower pathFollower = new PatrolPathFollower(nodeProximityDistance);
            MoveToDestinationNavigation moveToDestination = new MoveToDestinationNavigation(seeker);
            Timer tExecution = new Timer();
            
            statePatrol
                .SetUtility(utility)
                .AddCanEnter(tCooldown.IsReady) 
                .AddCanEnter(IsAnyPathAvailable)

                .AddOnBegin( () => tExecution.RestartRandom(executionTime))
                .AddOnBegin( inputHolder.ResetInput)
                .AddOnBegin( () =>
                {
                    SelectPatrolPath();
                    currentPatrolPathSettings.patrolPath.StartTraversal();

                    pathFollower.StartPathFollowing(controller.transform.position, currentPatrolPathSettings.patrolPath.GetPatrolPath(), 
                        currentPatrolPathSettings.traversalDirection, startingPathPointId);
                    moveToDestination.SetDestination(pathFollower.CurrentPoint);
                })
                .AddOnUpdate( () =>
                {
                    bool pathPointChanged = pathFollower.ProceedPathFollowing(controller.transform.position);

                    if ( pathPointChanged )
                    {
                        moveToDestination.SetDestination(pathFollower.CurrentPoint);
                    }
                    inputHolder.positionInput = moveToDestination.ToDestination(0.0f, 0.75f);
                })
                .AddOnEnd( () =>
                {
                    pathFollower.AbortPathFollowing();

                    // restart cooldowns
                    tCooldown.Restart();
                    currentPatrolPathSettings.tCooldown.Restart();
                    currentPatrolPathSettings.patrolPath.EndTraversal();
                })
                .AddShallReturn( tExecution.IsReady)
            ;   

        }
    }
}
