using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;
using System;
using BarkSystem;

namespace Ai
{
    [CreateAssetMenu(fileName = "BehaviourPackIdle", menuName = "Ris/Ai/BehaviourPack/Idle", order = 0)]
    public class BehaviourPackIdle : BehaviourPack
    {

        protected override void DefineBehaviours_Impl()
        {
            // references
            var monoAttentionSelector = controller.GetComponentInChildren<MonoAttentionSelector>();
            var inputHolder = controller.GetComponentInParent<InputHolder>();
            var seeker = controller.GetComponentInChildren<Pathfinding.Seeker>();

            // Attention mode aliases
            AttentionMode modeIdle = monoAttentionSelector.idleMode;

            // states
            var stateIdle = modeIdle.AddNewState("Idle");
            var stateLookAround = modeIdle.AddNewState("LookAround");

            // utility components
            Timer tState = new Timer(); // time current state has been running

            var lookAround = new LookAround(
                tChangeAngle: new RangedFloat(1.1f, 1.35f),
                desiredAngleDifference: new RangedFloat(40.0f,50.0f),
                rotationLerp: 0.02f
            );

            lookAround.Init(controller.transform);
            lookAround.SetState(stateLookAround, inputHolder);

            stateIdle
                .SetUtility(1)
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnBegin(() => tState.RestartRandom(0.2f, 1.5f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddCanTransitionToSelf(() => !tState.IsReady())
                .AddShallReturn(tState.IsReady)
            ;

            stateLookAround
                    .AddCanEnter(() => false)
                    .SetUtility(() => 0.35f)
                    .AddOnBegin(inputHolder.ResetInput)
                    .AddOnBegin(() => tState.RestartRandom(1.9f, 2.15f))
                    .AddOnBegin(inputHolder.ResetInput)
                    .AddShallReturn(tState.IsReady)
                ;
        }
    }
}
