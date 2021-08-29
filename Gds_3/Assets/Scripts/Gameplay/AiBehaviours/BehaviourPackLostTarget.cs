using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Random = UnityEngine.Random;
using UnityEngine;

using BarkSystem;

namespace Ai
{
    [CreateAssetMenu(fileName = "BehaviourPackLostTarget", menuName = "Ris/Ai/BehaviourPack/LostTarget", order = 0)]
    public class BehaviourPackLostTarget : BehaviourPack
    {
        public EStimuliFilterType filterType = EStimuliFilterType.EEnemy;
        public EAwarenessLevel awarenessLevel = EAwarenessLevel.EInaccurate;

        [Serializable]
        public class WalkRandomlySettings
        {
            public bool enabled = true;
            public RangedFloat executionTime = new RangedFloat(1.25f, 2.5f);
            public RangedFloat walkDistance = new RangedFloat(5, 10);
            public RangedFloat randomAngle = new RangedFloat(50, 170);
        }
        public WalkRandomlySettings walkRandomlySettings;

        [Serializable]
        public class FollowDirectionSettings
        {
            public bool enabled = true;
            public RangedFloat executionTime = new RangedFloat(2.25f, 4.5f);
            public RangedFloat randomAngle = new RangedFloat(50, 170);
        }
        public FollowDirectionSettings followDirectionSettings;

        [Serializable]
        public class LookAtSettings
        {
            public bool enabled = true;
            public RangedFloat executionTime = new RangedFloat(0.25f, 0.45f);
        }
        public LookAtSettings lookAtSettings;

        [Serializable]
        public class LookAroundSettings
        {
            public bool enabled = true;
            public RangedFloat executionTime = new RangedFloat(1.9f, 2.15f);
            public float cd = 1.0f;
            [Space]
            public RangedFloat tChangeAngle = new RangedFloat(0.8f, 0.9f);
            public RangedFloat desiredAngleDifference = new RangedFloat(70.0f, 140.0f);
            public float rotationLerp = 0.175f;
        }
        public LookAroundSettings lookAroundSettings;

        protected override void DefineBehaviours_Impl()
        {
            // references
            var monoAttentionSelector = controller.GetComponentInChildren<MonoAttentionSelector>();
            var inputHolder = controller.GetComponentInParent<InputHolder>();
            var seeker = controller.GetComponentInChildren<Pathfinding.Seeker>();
            var enemyFilter = GetEnemyFilter();
            var barkInstance = controller.GetComponent<BarkInstance>();

            // Attention mode aliases
            AttentionMode modeEnemyLost = monoAttentionSelector.GetAttentionMode(filterType, awarenessLevel);

            // utility components
            Timer tState = new Timer(); // time current state has been running


            // states
            var stateIdle = modeEnemyLost.AddNewState("Idle"); // in case none of the states can enter

            var stateLookAt = modeEnemyLost.AddNewState("LookAt");
            var stateLookAround = modeEnemyLost.AddNewState("LookAround");
            var stateWalkRandomly = modeEnemyLost.AddNewState("WalkRandomly");
            var stateFollowDirection = modeEnemyLost.AddNewState("FollowTargetDirection");

            var walkRandomly = new WalkRandomly(walkRandomlySettings.walkDistance);
            walkRandomly.Init(controller.transform);

            var moveToDestination = new MoveToDestinationNavigation(seeker);

            Timer walkRandomlyCd = new Timer();
            stateWalkRandomly
                .AddCanEnter(() => walkRandomlySettings.enabled)
                .SetUtility(1.75f)
                .AddOnEnd(walkRandomlyCd.Restart)
                .AddOnBegin(() => tState.RestartRandom(walkRandomlySettings.executionTime))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnBegin(() =>
                {
                    Vector3 center = enemyFilter.GetTargetPosition();

                    float maxRadius = walkRandomly.walkDistance.max + UtilityMathHelpers.ScallingUpTo(enemyFilter.elapsedTime, 1, 10);
                    RangedFloat dist = new RangedFloat(walkRandomly.walkDistance.min, maxRadius);

                    float currentForwardAngle = Vector2.SignedAngle(Vector3.forward.To2D(), transform.forward.To2D());

                    float distance = dist.GetRandom();
                    Vector3 offset = Quaternion.Euler(0, currentForwardAngle + walkRandomlySettings.randomAngle.GetRandomSigned(), 0) * Vector3.forward * distance;

                    walkRandomly.currentDestination = center + offset;
                })
                .AddOnUpdate(() =>
                {
                    inputHolder.positionInput = moveToDestination.ToDestinationAdaptiveLerp(
                        walkRandomly.currentDestination, 0.1f, 0.75f, 0.75f, 0.25f);
                })
                .AddShallReturn(tState.IsReady)
                .AddShallReturn(() => walkRandomly.IsCloseToDestination(0.75f) && tState.IsReady(1.0f))
                .AddCanTransitionToSelf(() => true)
            ;

            stateFollowDirection
                .AddCanEnter(() => followDirectionSettings.enabled)
                .SetUtility(() => 1.5f + UtilityMathHelpers.ConditionalFactor(enemyFilter.elapsedTime < 6.0f, 4.0f) )
                .AddOnEnd(walkRandomlyCd.Restart)
                .AddOnBegin(() => tState.RestartRandom(followDirectionSettings.executionTime))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnBegin(() =>
                {
                    var target = enemyFilter.GetTarget();
                    Vector3 targetPosition =  target.GetPositionNotScaled(7.0f, 0.75f);

                    DebugUtilities.DrawCross(targetPosition, 2, Color.red);

                    Vector3 center = targetPosition;

                    float maxRadius = walkRandomly.walkDistance.max + UtilityMathHelpers.ScallingUpTo(enemyFilter.elapsedTime, 1, 10);
                    RangedFloat dist = new RangedFloat(walkRandomly.walkDistance.min, maxRadius);

                    float currentForwardAngle = Vector2.SignedAngle(Vector3.forward.To2D(), transform.forward.To2D());

                    float distance = dist.GetRandom();
                    Vector3 offset = Quaternion.Euler(0, currentForwardAngle + followDirectionSettings.randomAngle.GetRandomSigned(), 0) * Vector3.forward * distance;

                    walkRandomly.currentDestination = center + offset;
                })
                .AddOnUpdate(() =>
                {
                    inputHolder.positionInput = moveToDestination.ToDestinationAdaptiveLerp(
                        walkRandomly.currentDestination, 0.1f, 0.75f, 0.75f, 0.25f);
                })
                .AddShallReturn(tState.IsReady)
                .AddShallReturn(() => walkRandomly.IsCloseToDestination(0.75f) && tState.IsReady(1.0f))
                .AddCanTransitionToSelf(() => true)
            ;


            stateIdle
                .SetUtility(0.001f)
                .AddOnBegin(inputHolder.ResetInput)
                .AddShallReturn(() => true);
            ;

            var lookAt = new LookAt(transform);

            stateLookAt
                .AddCanEnter(() => lookAtSettings.enabled)
                .SetUtility(() => UtilityMathHelpers.ScallingUpTo(enemyFilter.elapsedTime.Sq(), 0.005f, 0.25f))
                .AddCanEnter(() => RangedFloat.InRange(enemyFilter.elapsedTime, 1.25f, 7.5f))
                .AddCanEnter(() => enemyFilter.IsFurther(6) )
                .AddOnBegin(() => tState.RestartRandom(lookAtSettings.executionTime))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnBegin(() => lookAt.SetDestination(enemyFilter.GetTarget()))
                .AddOnUpdate(() => inputHolder.rotationInput = lookAt.UpdateRotationInput())
                .AddShallReturn(tState.IsReady)
            ;



            var lookAround = new LookAround(
                tChangeAngle: lookAroundSettings.tChangeAngle,
                desiredAngleDifference: lookAroundSettings.desiredAngleDifference,
                rotationLerp: lookAroundSettings.rotationLerp
            );
            lookAround.Init(controller.transform);
            lookAround.SetState(stateLookAround, inputHolder);
            Timer tLookAroundCd = new Timer(lookAroundSettings.cd);
            stateLookAround
                .AddCanEnter(() => lookAroundSettings.enabled)
                .SetUtility(() =>
                {
                    return 0.075f 
                        + UtilityMathHelpers.ScallingUpTo(enemyFilter.elapsedTime.Sq(), 0.005f, 0.15f) 
                        + UtilityMathHelpers.ConditionalFactor(walkRandomlyCd.IsReady(), 1);
                })
                .AddCanEnter(() => enemyFilter.elapsedTime > 1.85f)
                .AddOnBegin(() => tState.RestartRandom(lookAroundSettings.executionTime))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnEnd(tLookAroundCd.Restart)
                .AddCanEnter(tLookAroundCd.IsReady)
                .AddShallReturn(tState.IsReady)
            ;
        }
    }
}
