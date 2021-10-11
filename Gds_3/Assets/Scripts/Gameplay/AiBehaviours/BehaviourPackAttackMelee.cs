using UnityEngine;

namespace Ai
{
    [CreateAssetMenu(fileName = "BehaviourPackAttackMelee", menuName = "Ris/Ai/BehaviourPack/AttackMelee", order = 1)]
    public class BehaviourPackAttackMelee : BehaviourPack
    {
        [SerializeField] [Range(0.0f, 20.0f)] float chargeAttackTriggerDistance;
        [SerializeField] [Range(0.0f, 20.0f)] float areaAttackTriggerDistance;
        protected override void DefineBehaviours_Impl()
        {
            #region References and components
            // references
            var monoAttentionSelector = controller.GetComponentInChildren<MonoAttentionSelector>();
            var inputHolder = controller.GetComponentInParent<InputHolder>();
            var seeker = controller.GetComponentInChildren<Pathfinding.Seeker>();
            var enemyFilter = GetEnemyFilter();
            var animationStateMachine = inputHolder.GetComponentInChildren<AnimationStateMachine>();

            // Attention mode aliases
            AttentionMode modeEnemySeen = monoAttentionSelector.GetAttentionMode(EStimuliFilterType.EEnemy, EAwarenessLevel.ESeen);


            // utility components
            Timer tState = new Timer(); // time current state has been running
            Timer attackCooldown = new Timer(0.5f);
            #endregion References and components
            #region States init
            // states
            var stateIdle = modeEnemySeen.AddNewState("Idle"); // in case none of the states can enter

            var stateLookAt = modeEnemySeen.AddNewState("LookAt");
            var stateMoveTo = modeEnemySeen.AddNewState("MoveTo");
            var stateAttack = modeEnemySeen.AddNewState("Attack");
            var stateAreaAttack = modeEnemySeen.AddNewState("BigAreaAttack");
            var stateDashAway = modeEnemySeen.AddNewState("DashAway");
            var stateDashTowards = modeEnemySeen.AddNewState("DashTowards");
            var stateMoveBehind = modeEnemySeen.AddNewState("MoveBehind");
            var stateMoveSide = modeEnemySeen.AddNewState("MoveSide");
            var stateMoveAway = modeEnemySeen.AddNewState("MoveAway");
            #endregion States init

            #region Agressivenes init
            BoxValue<float> agressivenes = new BoxValue<float>(0.0f);

            var visualLogger = controller.GetComponentInParent<VisualLogger>();
            VisualLogger.LogData agressivenessLog = null;
            if (visualLogger)
            {
                agressivenessLog = visualLogger.AddLog(new Vector2(-100, -100), new Color(150, 00, 200));
            }

            stateMachine.AddOnUpdate((s) => {
                //agressivenes.value += ((Random.value * 2) - 1) * 0.025f;

                agressivenes.value = Mathf.Clamp01(agressivenes.value);
                if (agressivenessLog != null)
                {
                    agressivenessLog.text = agressivenes.value.ToString("N1");
                }
            });
            #endregion Agressivenes init

            //States logic
            stateIdle
                .SetUtility(0.001f)
                .AddOnBegin(inputHolder.ResetInput)
                .AddShallReturn(() => true);
            ;

            Herd herd = blackboard.GetValue<Herd>("herd").value; 

            var lookAt = new LookAt(transform);

            stateLookAt
                //.AddCanEnter(() => false)
                .AddOnBegin(inputHolder.ResetInput)
                //.SetUtility(() => UtilityMathHelpers.ScallingUpTo(enemyFilter.GetTarget().elapsedTime, 0.5f, 3.5f))
                .SetUtility(() => 30.0f)
                .AddOnUpdate(() =>
                {
                    lookAt.SetDestination(enemyFilter.GetTarget());
                    inputHolder.rotationInput = inputHolder.directionInput = lookAt.UpdateRotationInput();
                })

                .AddOnBegin(() => tState.RestartRandom(0.3f, 0.75f))
                .AddShallReturn(tState.IsReady)
                .AddShallReturn(() => true);
            ;


            modeEnemySeen.AddOnUpdate(() => Interrupt(stateAttack, () => Random.value <= UtilityMathHelpers.ConditionalFactor(enemyFilter.IsFurther(2.5f), 0.2f, 0.025f) ));
            stateAttack
                //.AddCanEnter(() => false)
                //.SetUtility(() => 0.5f)
                .SetUtility(() => 0 /*+ 0.35f * UtilityMathHelpers.ConditionalFactor(enemyFilter.IsFurther(3.5f), 0.0f, 1.0f)*/)

                .AddCanEnter(() => enemyFilter.IsCloser(chargeAttackTriggerDistance) && enemyFilter.elapsedTime < 0.5f)
                .AddCanEnter(attackCooldown.IsReady)
                .AddCanEnter(() => animationStateMachine.GetState(4).CanEnter())

                .AddOnBegin(inputHolder.ResetInput)
                .AddOnBegin(() =>
                {
                    //Vector3 targetPosition = enemyFilter.GetTarget().GetPositionNotScaled(float.MaxValue, 1f);
                    Vector3 targetPosition = enemyFilter.GetTarget().position;

                    Vector2 desiredDirectionInput = (targetPosition - transform.position).To2D();
                    inputHolder.directionInput = desiredDirectionInput;
                })

                .AddOnUpdate(() =>
                {
                    inputHolder.keys[4] = !tState.IsReady(0.1f);
                })

                .AddOnEnd(attackCooldown.Restart)

                .AddShallReturn(() => tState.IsReady(0.1f) && animationStateMachine.currentState != animationStateMachine.GetState(4))
                .AddShallReturn(() => animationStateMachine.animationTime >= 0.6f)
                .AddCanBeInterrupted(() => false)
            ;


            modeEnemySeen.AddOnUpdate(() => Interrupt(stateAreaAttack, () => Random.value <= UtilityMathHelpers.ConditionalFactor(enemyFilter.IsCloser(2.5f), 0.075f, 0.025f)));
            stateAreaAttack
                //.AddCanEnter(() => false)
                //.SetUtility(() => 1.25f)
                .SetUtility(() => 0)

                .AddCanEnter(() => enemyFilter.IsCloser(areaAttackTriggerDistance) && enemyFilter.elapsedTime < 0.5f)
                .AddCanEnter(() => animationStateMachine.GetState(3).CanEnter())
                .AddCanEnter(attackCooldown.IsReady)
                .AddOnBegin(() => tState.RestartRandom(0.3f, 0.5f))
                .AddOnBegin(inputHolder.ResetInput)

                .AddOnUpdate(() =>
                {
                    inputHolder.keys[3] = !tState.IsReady(0.1f);
                    Vector2 desiredDirectionInput = (enemyFilter.GetTarget().position - transform.position).To2D();
                    inputHolder.directionInput = desiredDirectionInput;// Vector2.Lerp(inputHolder.directionInput, desiredDirectionInput, 0.1f);
                    //inputHolder.positionInput = desiredDirectionInput;
                })

                .AddOnEnd(attackCooldown.Restart)

                .AddShallReturn(() => tState.IsReady(0.1f) && animationStateMachine.currentState != animationStateMachine.GetState(3))
                .AddShallReturn(() => animationStateMachine.animationTime >= 0.6f)
                .AddCanBeInterrupted(() => false)
            ;


            var moveToDestination = new MoveToDestinationNavigation(seeker);
            Vector3 influence = Vector3.zero;

            stateMoveSide
                //.AddCanEnter(() => false)
                .SetUtility(() => 0.25f)

                .AddOnBegin(() => tState.RestartRandom(0.4f, 0.5f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnBegin(() =>
                {
                    Vector3 toTarget = enemyFilter.ToTargetSide() * 2;
                    Vector3 target = toTarget + transform.position;

                    moveToDestination.SetDestination(target);
                })

                .AddOnUpdate(() => DebugUtilities.DrawCross(moveToDestination.destination, 2, Color.red))
                .AddOnUpdate(() =>
                {
                    inputHolder.positionInput = moveToDestination.toDestination.To2D();
                    inputHolder.directionInput = enemyFilter.ToTarget2D();
                })


                .AddShallReturn(tState.IsReady)
            ;

            stateMoveBehind
                //.AddCanEnter(() => false)

                .SetUtility(() => 0.35f)
                .AddCanEnter(() => !enemyFilter.IsAtBehindTargetPosition(1.5f))
                
                .AddOnBegin(() => tState.RestartRandom(0.4f, 0.8f))
                .AddOnBegin(inputHolder.ResetInput)
                
                .AddOnUpdate(() => DebugUtilities.DrawCross(enemyFilter.BehindTargetPosition(), 2, Color.red))
                .AddOnUpdate(() =>
                {
                    Vector3 toTarget = enemyFilter.StayBehindTargetAdaptive(1.0f, 600, 1f);
                    Vector3 target = toTarget + transform.position;

                    DebugUtilities.DrawCross(target, 2, Color.blue);

                    float f = 3.0f;
                    influence = Vector3.MoveTowards(influence, herd.GetOutDirection(controller, f), 0.0001f);

                    //inputHolder.positionInput = enemyFilter.StayBehindTargetAdaptive2D(1.0f, 400, 0.75f);
                    inputHolder.positionInput = moveToDestination.ToDestinationAdaptiveLerp(target + influence.normalized * f, 0.1f, 1.0f, 0.75f, 2.25f);
                    inputHolder.directionInput = enemyFilter.ToTarget2D();
                })
                
                .AddShallReturn(tState.IsReady)
            ;

            stateMoveTo
                //.AddCanEnter(() => false)
                //.AddCanEnter(() => agressivenes.value >= 0.5)
                //.SetUtility(() => 0.25f + UtilityMathHelpers.ConditionalFactor(Random.value < agressivenes.value*0.5f, 3.0f))
                /*.SetUtility(() =>
                    agressivenes.value * 2
                    + UtilityMathHelpers.ConditionalFactor(enemyFilter.IsInRange(0.0f, 2 * chargeAttackTriggerDistance), 2.0f)
                    + UtilityMathHelpers.ScallingUpTo(enemyFilter.GetTarget().elapsedTime, 0.5f, 1.5f)
                )*/
                .SetUtility(1.0f)
                
                .AddOnBegin(() => tState.RestartRandom(0.75f, 1.25f))
                .AddOnBegin(inputHolder.ResetInput)

                .AddOnUpdate(() =>
                {
                    float f = 3.0f;
                    influence = Vector3.MoveTowards(influence, herd.GetOutDirection(controller, f), 0.0001f);

                    inputHolder.positionInput = moveToDestination.ToDestinationAdaptiveLerp(
                        enemyFilter.GetTarget().position + influence.normalized * f, 0.1f, 2.0f, 0.75f, 0.25f);
                })

                .AddShallReturn(tState.IsReady)
                .AddShallReturn(() => enemyFilter.IsCloser(2.0f))
            ;

            stateMoveAway
                .AddCanEnter(() => false)
                //.AddCanEnter(() => agressivenes.value >= 0.5)
                //.SetUtility(() => 0.25f + UtilityMathHelpers.ConditionalFactor(Random.value < agressivenes.value*0.5f, 3.0f))
                /*.SetUtility(() =>
                    agressivenes.value * 2
                    + UtilityMathHelpers.ConditionalFactor(enemyFilter.IsInRange(0.0f, 2 * chargeAttackTriggerDistance), 2.0f)
                    + UtilityMathHelpers.ScallingUpTo(enemyFilter.GetTarget().elapsedTime, 0.5f, 1.5f)
                )*/
                .SetUtility(0.125f)

                .AddOnBegin(() => tState.RestartRandom(0.75f, 1.0f))
                .AddOnBegin(inputHolder.ResetInput)

                .AddOnUpdate(() =>
                {
                    inputHolder.positionInput = moveToDestination.ToDestinationAdaptiveLerp(
                        enemyFilter.GetAwayFromTargetPosition(10), 0.1f, 2.0f, 0.75f, 0.25f);
                })

                .AddShallReturn(tState.IsReady)
                .AddShallReturn(() => enemyFilter.IsCloser(2.0f))
            ;

            modeEnemySeen.AddOnUpdate(() => Interrupt(stateDashAway, () => Random.value <= 0.045f ));
            stateDashAway
                //.AddCanEnter(() => false)
                .SetUtility(() => 0)
                .AddCanEnter(() => animationStateMachine.GetState(2).CanEnter()  )
                .AddCanEnter(() => enemyFilter.IsCloser(3))
                .AddOnBegin(() => tState.RestartRandom(0.35f, 0.5f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnUpdate(() =>
                {
                    inputHolder.keys[1] = !tState.IsReady(0.1f);
                    Vector2 desiredDirectionInput = (enemyFilter.GetTarget().position - transform.position).To2D();
                    inputHolder.directionInput = -desiredDirectionInput;// Vector2.Lerp(inputHolder.directionInput, desiredDirectionInput, 0.1f);
                    inputHolder.positionInput = -desiredDirectionInput;
                    //inputHolder.rotationInput = desiredDirectionInput;
                })
                //.AddShallReturn(tState.IsReady)
                .AddShallReturn(() => tState.IsReady(0.1f) && animationStateMachine.currentState != animationStateMachine.GetState(2))
                .AddShallReturn(() => animationStateMachine.animationTime >= 0.7f)
                .AddCanBeInterrupted(() => false)
            ;
            /////////////////////////////////////

            stateDashTowards
                .AddCanEnter(() => false)
                .SetUtility(() => 0.6f + UtilityMathHelpers.ConditionalFactor(Random.value < agressivenes.value * 0.5f, 1.25f))
                .AddCanEnter(() => enemyFilter.IsCloser(8))
                .AddOnBegin(() => tState.RestartRandom(0.35f, 0.5f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnUpdate(() =>
                {
                    inputHolder.keys[1] = true;
                    Vector2 desiredDirectionInput = (enemyFilter.GetTarget().position - transform.position).To2D();
                    inputHolder.directionInput = desiredDirectionInput;// Vector2.Lerp(inputHolder.directionInput, desiredDirectionInput, 0.1f);
                    inputHolder.positionInput = desiredDirectionInput;
                    inputHolder.rotationInput = desiredDirectionInput;
                })
                .AddShallReturn(tState.IsReady)
            ;

            /*var stateIdle = modeIdle.AddNewState("Idle");
            var stateLookAround = modeIdle.AddNewState("LookAround");

            // utility components
            Timer tState = new Timer(); // time current state has been running

            var lookAround = new LookAround(
                tChangeAngle: new RangedFloat(1.1f, 1.35f),
                desiredAngleDifference: new RangedFloat(40.0f, 50.0f),
                rotationLerp: 0.02f
            );

            lookAround.Init(controller.transform);
            lookAround.SetState(stateLookAround, inputHolder);

            stateIdle
                .SetUtility(1)
                .AddOnBegin(() => tState.RestartRandom(0.2f, 1.5f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddCanTransitionToSelf(() => !tState.IsReady())
                .AddShallReturn(tState.IsReady)
            ;

            stateLookAround
                    .SetUtility(() => 0.35f)
                    .AddOnBegin(() => tState.RestartRandom(1.9f, 2.15f))
                    .AddOnBegin(inputHolder.ResetInput)
                    .AddShallReturn(tState.IsReady)
                ;*/
        }
    }
}
