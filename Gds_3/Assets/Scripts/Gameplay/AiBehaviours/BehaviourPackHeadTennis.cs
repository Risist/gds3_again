using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ai
{
    [CreateAssetMenu(fileName = "BehaviourPackHeadTennis", menuName = "Ris/Ai/BehaviourPack/HeadTennis", order = 1)]
    public class BehaviourPackHeadTennis : BehaviourPack
    {
        [Range(0, 1)] public float interruptionChanceFast = 1;
        [Range(0, 1)] public float interruptionChanceStationary = 1;
        public float utilityFast;
        public float utilityStationary;
        public float reflectCd;
        public float bulletDistanceToReflect = 4.5f;
        public float bulletVelocityToReflect = 4.5f;
        public int attackKeyId = 0;
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
            MinimalTimer tReflectCd = blackboard.InitValue("BulletReaction", () => new MinimalTimer()).value;

            // states
            var stateReflect = modeEnemySeen.AddNewState("Reflect the Projectile");

            modeEnemySeen.AddOnUpdate(() => Interrupt(stateReflect, 
                () =>  UnityEngine.Random.value <= (objectDetection.queries[objectDetectorQueryId].velocity > bulletVelocityToReflect ?
                        interruptionChanceFast : interruptionChanceStationary)));


            Timer walkRandomlyCd = new Timer();
            stateReflect
                //.AddCanEnter(() => false)
                .SetUtility(() => objectDetection.queries[objectDetectorQueryId].velocity > bulletVelocityToReflect ? utilityFast : utilityStationary)

                .AddCanEnter(() => animationStateMachine.GetState(1).CanEnter() || animationStateMachine.GetState(5).CanEnter())
                .AddCanEnter(() => tReflectCd.IsReady(reflectCd) )
                .AddCanEnter(() => objectDetection.queries[objectDetectorQueryId].closestDistance < bulletDistanceToReflect) // Detect incomming bullet

                .AddOnBegin(inputHolder.ResetInput)

                .AddOnEnd(tReflectCd.Restart)

                .AddOnUpdate(() =>
                {
                    inputHolder.keys[attackKeyId] = !tState.IsReady(0.1f);

                    // compute bullet position, direction to bullet and insert params
                    Vector3 closestColliderPosition = objectDetection.queries[objectDetectorQueryId].closestCollider.transform.position;

                    Vector2 desiredDirectionInput = (closestColliderPosition - transform.position).To2D();
                    inputHolder.directionInput = desiredDirectionInput;
                })
                .AddShallReturn(() => tState.IsReady(0.1f) && animationStateMachine.currentState != animationStateMachine.GetState(1))
                .AddShallReturn(() => animationStateMachine.animationTime >= 1.0f)
                .AddCanBeInterrupted(() => false)
            ;
        }
    }
}
