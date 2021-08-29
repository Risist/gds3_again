using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;
using System;

[CreateAssetMenu(fileName = "TestBehaviourPack", menuName = "Ris/Ai/BehaviourPack/Test", order = 0)]
public class TestBehaviourPack : BehaviourPack
{
    protected override void DefineBehaviours_Impl()
    {
        // references
        var monoAttentionSelector = controller.GetComponentInChildren<MonoAttentionSelector>();
        var inputHolder = controller.GetComponentInParent<InputHolder>();
        var seeker = controller.GetComponentInChildren<Pathfinding.Seeker>();
        var enemyFilter = GetEnemyFilter();

        // Attention mode aliases
        AttentionMode modeEnemySeen = monoAttentionSelector.GetAttentionMode(EStimuliFilterType.EEnemy, EAwarenessLevel.ESeen);
        AttentionMode modeEnemyLost = monoAttentionSelector.GetAttentionMode(EStimuliFilterType.EEnemy, EAwarenessLevel.EInaccurate);
        AttentionMode modeIdle      = monoAttentionSelector.idleMode;

        // states
        var stateIdle       = modeIdle     .AddNewState("Idle");
        var stateMoveTo     = modeEnemySeen.AddNewState("MoveTo");
        var stateLookAround = modeEnemyLost.AddNewState("LookAround - old event");


        // utility components
        Timer tState = new Timer(); // time current state has been running
        var moveToDestinationNavigation = new MoveToDestinationNavigation(seeker);
        var moveToDestination = new MoveToDestination(transform);

        stateIdle
                .SetUtility(1)
                .AddOnBegin(() => tState.RestartRandom(0.2f, 1.5f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddCanTransitionToSelf(() => !tState.IsReady())
                .AddShallReturn(() => true)
            ;

        stateMoveTo
                .SetUtility(() => 10000)
                .AddOnBegin(() => tState.RestartRandom(0.5f, 0.75f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnUpdate(() =>
                {
                    inputHolder.positionInput = moveToDestinationNavigation.ToDestinationAdaptiveLerp(
                        enemyFilter.GetTarget().position, 0.1f, 2.0f, 0.75f, 0.25f );
                })
                .AddShallReturn(tState.IsReady)
            ;

        /*var lookAround = new ComponentLookAround();
        lookAround.behaviourController = controller;
        lookAround.OnComponentAdd(stateLookAround);
        stateLookAround
                .SetUtility(() => 10000)
                .AddOnBegin(() => tState.RestartRandom(0.5f, 0.75f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddShallReturn(tState.IsReady)
            ;*/
    }
}
