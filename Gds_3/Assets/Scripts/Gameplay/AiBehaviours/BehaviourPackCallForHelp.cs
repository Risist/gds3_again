using UnityEngine;
using BarkSystem;

namespace Ai
{
    [CreateAssetMenu(fileName = "BehaviourPackCallFroHelp", menuName = "Ris/Ai/BehaviourPack/CallFroHelp", order = 0)]
    public class BehaviourPackCallForHelp : BehaviourPack
    {
        public EAwarenessLevel awarenessLevel = EAwarenessLevel.ESeen;
        public RangedFloat executionTime;
        public BarkSet bark;
        public float cd = 20;
        public float utility = 5;
        public float minEnemyDistance = 6;

        protected override void DefineBehaviours_Impl()
        {
            // references
            var monoAttentionSelector = controller.GetComponentInChildren<MonoAttentionSelector>();
            var inputHolder = controller.GetComponentInParent<InputHolder>();
            var seeker = controller.GetComponentInChildren<Pathfinding.Seeker>();
            var enemyFilter = GetEnemyFilter();
            var barkInstance = controller.GetComponent<BarkInstance>();

            // Attention mode aliases
            AttentionMode modeEnemyLost = monoAttentionSelector.GetAttentionMode(EStimuliFilterType.EEnemy, awarenessLevel);

            // utility components
            Timer tState = new Timer(); // time current state has been running
            Timer tCd = new Timer(cd);

            // states
            var callForHelp = modeEnemyLost.AddNewState("CallForHelp");

            var moveToDestination = new MoveToDestinationNavigation(seeker);

            callForHelp
                .SetUtility(utility)

                .AddCanEnter(() => enemyFilter.HasTarget() && enemyFilter.IsFurther(minEnemyDistance))
                .AddCanEnter(tCd.IsReady)
                .AddCanEnter(() => barkInstance.currentBarkController == null) // no current bark
                .AddCanEnter(NoiseManager.Instance.CanSpreadNoise)

                .AddOnBegin(() => tState.RestartRandom(executionTime))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnBegin(() => BarkManager.Instance.CallBark(transform, bark))
                .AddOnBegin(() =>
                {
                    NoiseData data = new NoiseData();
                    data.position = enemyFilter.GetTargetPosition().To2D();
                    data.velocity = enemyFilter.GetTarget().velocity.To2D();
                    
                    if (enemyFilter.GetTarget().perceiveUnit)
                    {
                        data.fraction = enemyFilter.GetTarget().perceiveUnit.fraction;
                    }
                    
                    NoiseManager.Instance.SpreadNoise(data);
                })
                .AddOnEnd( tCd.Restart )
                
                .AddShallReturn(tState.IsReady)
            ;
        }
    }
}
