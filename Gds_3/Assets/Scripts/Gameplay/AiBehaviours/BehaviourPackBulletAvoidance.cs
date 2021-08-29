using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Ai
{

    [CreateAssetMenu(fileName = "BehaviourPackBulletAvoidance", menuName = "Ris/Ai/BehaviourPack/BulletAvoidance", order = 0)]
    public class BehaviourPackBulletAvoidance : BehaviourPack
    {
        [Serializable]
        public class MovementSettings
        {
            [Range(0, 1)] public float interruptionChance = 1;
            public float utilityFast;
            public float utilityStationary;
            public float bulletDistanceToDash = 4.5f;
            public float bulletVelocityToDash = 4.5f;
        }

        public MovementSettings dashAway;
        public MovementSettings dashSide;
        public MovementSettings moveSide;
        public MovementSettings moveAway;


        public float dashCd;
        public int dashKeyId = 1;
        public int objectDetectorQueryId = 0;

        protected override void DefineBehaviours_Impl()
        {
            // references
            var monoAttentionSelector = controller.GetComponentInChildren<MonoAttentionSelector>();
            var inputHolder = controller.GetComponentInParent<InputHolder>();
            var objectDetection = controller.GetComponentInChildren<ObjectDetection>();
            var animationStateMachine = inputHolder.GetComponentInChildren<AnimationStateMachine>();

            // Attention mode aliases
            AttentionMode modeEnemySeen = monoAttentionSelector.GetAttentionMode(EStimuliFilterType.EEnemy, EAwarenessLevel.ESeen);

            // utility components
            Timer tState = new Timer(); // time current state has been running
            MinimalTimer tDashCd = blackboard.InitValue("BulletReaction", () => new MinimalTimer() ).value;

            // states
            var dashAwayFromBullet = modeEnemySeen.AddNewState("Dash Away From Projectile");
            var dashSideFromBullet = modeEnemySeen.AddNewState("Dash Side From Projectile");

            modeEnemySeen.AddOnUpdate(() => Interrupt(dashAwayFromBullet, 
                () => objectDetection.queries[objectDetectorQueryId].velocity > dashAway.bulletVelocityToDash 
                    && UnityEngine.Random.value <= dashAway.interruptionChance));
            modeEnemySeen.AddOnUpdate(() => Interrupt(dashSideFromBullet,
                () => objectDetection.queries[objectDetectorQueryId].velocity > dashSide.bulletVelocityToDash
                    && UnityEngine.Random.value <= dashSide.interruptionChance));


            dashAwayFromBullet
                //.AddCanEnter(() => false)
                .SetUtility(() => objectDetection.queries[objectDetectorQueryId].velocity > dashAway.bulletVelocityToDash ? dashAway.utilityFast : dashAway.utilityStationary)

                .AddCanEnter(() => animationStateMachine.GetState(2).CanEnter())
                .AddCanEnter(() => tDashCd.IsReady(dashCd))
                .AddCanEnter(() => objectDetection.queries[objectDetectorQueryId].closestDistance < dashAway.bulletDistanceToDash) // Detect incomming bullet

                .AddOnBegin(inputHolder.ResetInput)

                .AddOnEnd(tDashCd.Restart)

                .AddOnUpdate(() =>
                {
                    inputHolder.keys[dashKeyId] = !tState.IsReady(0.1f);

                    // compute bullet position, direction to bullet and insert params
                    Vector3 closestColliderPosition = objectDetection.queries[objectDetectorQueryId].closestCollider.transform.position;

                    Vector2 desiredDirectionInput = (closestColliderPosition - transform.position).To2D();
                    inputHolder.directionInput = -desiredDirectionInput;
                    inputHolder.positionInput = -desiredDirectionInput;
                })
                .AddShallReturn(() => tState.IsReady(0.1f) && animationStateMachine.currentState != animationStateMachine.GetState(2))
                .AddCanBeInterrupted( () => false)
            ;

            dashSideFromBullet
                //.AddCanEnter(() => false)
                .SetUtility(() => objectDetection.queries[objectDetectorQueryId].velocity > dashSide.bulletVelocityToDash ? dashSide.utilityFast : dashSide.utilityStationary)

                .AddCanEnter(() => animationStateMachine.GetState(2).CanEnter())
                .AddCanEnter(() => tDashCd.IsReady(dashCd))
                .AddCanEnter(() => objectDetection.queries[objectDetectorQueryId].closestDistance < dashSide.bulletDistanceToDash) // Detect incomming bullet

                .AddOnBegin(inputHolder.ResetInput)

                .AddOnEnd(tDashCd.Restart)

                .AddOnUpdate(() =>
                {
                    inputHolder.keys[dashKeyId] = true;

                    // compute bullet position, direction to bullet and insert params
                    Vector3 closestColliderPosition = objectDetection.queries[objectDetectorQueryId].closestCollider.transform.position;

                    Vector2 desiredDirectionInput = Vector2.Perpendicular((closestColliderPosition - transform.position).To2D());
                    desiredDirectionInput *= UnityEngine.Random.value < 0.5f ? 1 : -1; 

                    inputHolder.directionInput = desiredDirectionInput;
                    inputHolder.positionInput = desiredDirectionInput;
                })
                .AddShallReturn(() => tState.ElapsedTime() > 0.1f && animationStateMachine.currentState != animationStateMachine.GetState(2))
                .AddCanBeInterrupted(() => false)
            ;
        }
    }
}
