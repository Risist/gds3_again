using UnityEngine;

namespace Ai
{
    [CreateAssetMenu(fileName = "BehaviourPackAttackSimple", menuName = "Ris/Ai/BehaviourPack/AttackSimple", order = 1)]
    public class BehaviourPackAttackSimple: BehaviourPack
    {
        [SerializeField] [Range(0.0f, 5.0f)] float attackTriggerDistance;
        [SerializeField] [Range(0.0f, 5.0f)] float attackCooldown = 1.0f;
        protected override void DefineBehaviours_Impl()
        {
            #region References and components
            // references
            var monoAttentionSelector = controller.GetComponentInChildren<MonoAttentionSelector>();
            var inputHolder = controller.GetComponentInParent<InputHolder>();
            var seeker = controller.GetComponentInChildren<Pathfinding.Seeker>();
            var enemyFilter = GetEnemyFilter();

            // Attention mode aliases
            AttentionMode modeEnemySeen = monoAttentionSelector.GetAttentionMode(EStimuliFilterType.EEnemy, EAwarenessLevel.ESeen);


            // utility components
            Timer tState = new Timer(); // time current state has been running
            Timer tAttackCooldown = new Timer(attackCooldown);
            int attackCounter = 0;
            #endregion References and components
            #region States init
            // states
            var stateIdle = modeEnemySeen.AddNewState("Idle"); // in case none of the states can enter

            var stateLookAt = modeEnemySeen.AddNewState("LookAt");
            var stateMoveTo = modeEnemySeen.AddNewState("MoveTo");
            var stateAttack = modeEnemySeen.AddNewState("Attack");
            var stateWhack = modeEnemySeen.AddNewState("Whack");
            var stateDashAway = modeEnemySeen.AddNewState("DashAway");
            var stateDashTowards = modeEnemySeen.AddNewState("DashTowards");
            var stateMoveBehind = modeEnemySeen.AddNewState("MoveBehind");
            var stateMoveSide = modeEnemySeen.AddNewState("MoveSide");
            #endregion States init
            #region Agressivenes init
            BoxValue<float> agressivenes = new BoxValue<float>(0.0f);

            var visualLogger = controller.GetComponentInParent<VisualLogger>();
            VisualLogger.LogData agressivenessLog = null;
            if (visualLogger)
            {
                agressivenessLog = visualLogger.AddLog(new Vector2(-100, -100), new Color(150, 00, 200) );
            }

            stateMachine.AddOnUpdate((s) => {
                agressivenes.value += ((Random.value * 2) - 1) * Time.deltaTime;

                agressivenes.value = Mathf.Clamp(agressivenes.value, -1.0f, 1.0f);
                if(agressivenessLog != null)
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

            var lookAt = new LookAt(transform);

            stateLookAt
                //.AddCanEnter(() => false)
                .AddOnBegin(inputHolder.ResetInput)
                //.SetUtility(() => UtilityMathHelpers.ScallingUpTo(enemyFilter.GetTarget().elapsedTime, 0.5f, 3.5f))
                .SetUtility(() => 1.0f)
                .AddOnBegin(() => lookAt.SetDestination(enemyFilter.GetTarget()))
                .AddOnUpdate(() => inputHolder.rotationInput = lookAt.UpdateRotationInput())
                .AddOnUpdate(() =>
                {
                    agressivenes.value += 2.0f * Time.deltaTime;
                })
                .AddOnBegin(() => tState.RestartRandom(0.5f, 1.0f))
                .AddShallReturn(tState.IsReady)
                .AddShallReturn(() => agressivenes.value >= 0.5f)
                //.AddShallReturn(() => true);
            ;

            var moveToDestination = new MoveToDestinationNavigation(seeker);

            stateMoveTo
                .AddCanEnter(() => agressivenes.value >= 0.5)
                //.SetUtility(() => 0.25f + UtilityMathHelpers.ConditionalFactor(Random.value < agressivenes.value*0.5f, 3.0f))
                .SetUtility(() =>
                    agressivenes.value * 2
                    + UtilityMathHelpers.ConditionalFactor(enemyFilter.IsInRange(0.0f, 2*attackTriggerDistance), 2.0f)
                    + UtilityMathHelpers.ScallingUpTo(enemyFilter.GetTarget().elapsedTime, 0.5f, 1.5f)
                )
                .AddOnBegin(() => tState.RestartRandom(0.5f, 0.75f))
                //.AddOnBegin(() => tState.Restart(0.1f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnUpdate(() =>
                {
                    inputHolder.positionInput = moveToDestination.ToDestinationAdaptiveLerp(
                        enemyFilter.GetTarget().position, 0.1f, 2.0f, 0.75f, 0.25f);
                })
                .AddShallReturn(tState.IsReady)
                .AddShallReturn(() => enemyFilter.IsCloser(attackTriggerDistance))
            ;

            stateMoveSide
                .AddCanEnter(() => false)
                .SetUtility(() => 2.5f - agressivenes.value)
                .AddCanEnter(() => agressivenes.value < 0.0f)
                .AddOnBegin(() => tState.RestartRandom(0.5f, 0.8f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnBegin(() =>
                {
                    Vector3 toTarget = enemyFilter.ToTargetSide() * 2;
                    Vector3 target = toTarget + transform.position;


                    moveToDestination.SetDestination(target);
                })

                //.AddOnUpdate(() => DebugUtilities.DrawCross(enemyFilter.BehindTargetPosition(), 2, Color.red))
                .AddOnUpdate(() =>
                {
                    inputHolder.positionInput = moveToDestination.toDestination;
                })
                .AddShallReturn(tState.IsReady)
            ;

            stateMoveBehind
                //.AddCanEnter(() => false)
                //.AddCanEnter( () => !enemyFilter.IsAtBehindTargetPosition(1.5f))
                .SetUtility(() => 2.5f - agressivenes.value)
                .AddCanEnter(() => agressivenes.value < 0.0f)
                .AddOnBegin(() => tState.RestartRandom(0.4f, 0.8f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnUpdate(() => DebugUtilities.DrawCross(enemyFilter.BehindTargetPosition(), 2, Color.red))
                .AddOnUpdate(() =>
                {
                    Vector3 toTarget = enemyFilter.StayBehindTargetAdaptive(1.0f, 600, 1f);
                    Vector3 target = toTarget + transform.position;
                    agressivenes.value += 0.5f * Time.deltaTime;
                    DebugUtilities.DrawCross(target, 2, Color.blue);

                    //inputHolder.positionInput = enemyFilter.StayBehindTargetAdaptive2D(1.0f, 400, 0.75f);
                    inputHolder.positionInput = moveToDestination.ToDestinationAdaptiveLerp(target, 0.1f, 1.0f, 0.75f, 2.25f);
                })
                .AddShallReturn(tState.IsReady)
            ;

            stateAttack
                //.AddCanEnter(() => false)
                .SetUtility(() => 0 + (enemyFilter.IsCloser(attackTriggerDistance) ? 90 : 0))
                .AddCanEnter(() => enemyFilter.IsCloser(4) && enemyFilter.elapsedTime < 0.5f)
                .AddCanEnter(tAttackCooldown.IsReady)
                .AddOnBegin(() => tState.RestartRandom(0.5f, 0.75f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnEnd(tAttackCooldown.Restart)
                .AddOnEnd(() =>
                {
                    agressivenes.value = 0.0f;
                    attackCounter++;
                })
                .AddOnUpdate(() =>
                {
                    inputHolder.keys[0] = true;
                    Vector2 desiredDirectionInput = (enemyFilter.GetTarget().position - transform.position).To2D();
                    inputHolder.directionInput = desiredDirectionInput;// Vector2.Lerp(inputHolder.directionInput, desiredDirectionInput, 0.1f);
                    inputHolder.positionInput = desiredDirectionInput;
                })
                .AddShallReturn(tState.IsReady)
            ;

            stateWhack
                //.AddCanEnter(() => false)
                .SetUtility(() =>
                    
                       90 * (attackCounter - 1)
                       + 90 * (attackTriggerDistance / enemyFilter.ToTarget2D().magnitude)
                    )
                .AddCanEnter(() => enemyFilter.IsCloser(4) && enemyFilter.elapsedTime < 0.5f)
                .AddCanEnter(() => agressivenes.value >= 0.5f)
                .AddCanEnter(() => attackCounter >= 1)
                .AddCanEnter(tAttackCooldown.IsReady)
                .AddOnBegin(() => tState.RestartRandom(1.0f, 1.1f))
                .AddOnBegin(() => 
                {
                    inputHolder.ResetInput();
                    Vector2 desiredDirectionInput = (enemyFilter.GetTarget().position - transform.position).To2D();
                    inputHolder.rotationInput = desiredDirectionInput;
                })
                .AddOnEnd(tAttackCooldown.Restart)
                .AddOnEnd(() =>
                {
                    agressivenes.value = -1.0f;
                    attackCounter = 0;
                })
                .AddOnUpdate(() =>
                {
                    if(tState.ElapsedTime() < tState.cd / 3.0f)
                    {
                        Vector2 desiredDirectionInput = (enemyFilter.GetTarget().position - transform.position).To2D();
                        inputHolder.rotationInput = desiredDirectionInput;
                    }

                    inputHolder.keys[3] = true;
                    //Vector2 desiredDirectionInput = (enemyFilter.GetTarget().position - transform.position).To2D();
                    //inputHolder.directionInput = desiredDirectionInput;// Vector2.Lerp(inputHolder.directionInput, desiredDirectionInput, 0.1f);
                    //inputHolder.rotationInput = desiredDirectionInput;
                    //inputHolder.positionInput = desiredDirectionInput;
                })
                .AddShallReturn(tState.IsReady)
            ;

            stateDashAway
                .AddCanEnter(() => false)
                .SetUtility(() => 0.75f + UtilityMathHelpers.ConditionalFactor(Random.value < (1 - agressivenes.value * 0.5f), 1.25f))
                .AddCanEnter(() => enemyFilter.IsCloser(6) )
                .AddOnBegin(() => tState.RestartRandom(0.35f, 0.5f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnUpdate(() =>
                {
                    inputHolder.keys[1] = true;
                    Vector2 desiredDirectionInput = (enemyFilter.GetTarget().position - transform.position).To2D();
                    inputHolder.directionInput = -desiredDirectionInput;// Vector2.Lerp(inputHolder.directionInput, desiredDirectionInput, 0.1f);
                    inputHolder.positionInput = -desiredDirectionInput;
                    inputHolder.rotationInput = desiredDirectionInput;
                })
                .AddShallReturn(tState.IsReady)
            ;
            
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
