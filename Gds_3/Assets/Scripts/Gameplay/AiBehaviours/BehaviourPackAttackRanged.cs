using UnityEngine;

namespace Ai
{
    [CreateAssetMenu(fileName = "BehaviourPackAttackRanged", menuName = "Ris/Ai/BehaviourPack/AttackRanged", order = 0)]
    public class BehaviourPackAttackRanged : BehaviourPack
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


            // utility components
            Timer tState = new Timer(); // time current state has been running


            // states
            var stateIdle = modeEnemySeen.AddNewState("Idle"); // in case none of the states can enter
            
            var stateAttack = modeEnemySeen.AddNewState("Attack");
            var stateDashAway = modeEnemySeen.AddNewState("DashAway");
            var stateMoveAway = modeEnemySeen.AddNewState("MoveAway"); 
            var stateLookAt = modeEnemySeen.AddNewState("LookAt");
            var stateMoveBehind = modeEnemySeen.AddNewState("MoveBehind");

            controller.blackboard.GetValue<Herd>("herd");
            /*
            var stateMoveTo = modeEnemySeen.AddNewState("MoveTo");
            
            var stateDashTowards = modeEnemySeen.AddNewState("DashTowards");
            var stateMoveSide = modeEnemySeen.AddNewState("MoveSide");*/

            stateIdle
                .SetUtility(0.001f)
                .AddOnBegin(inputHolder.ResetInput)
                .AddShallReturn(() => true);
            ;

            Timer tShoot = new Timer(0.0f);
            stateAttack
                //.AddCanEnter(() => false)
                .SetUtility(() => 10)
                .AddCanEnter(tShoot.IsReady)
                .AddCanEnter(() => enemyFilter.IsInRange(4, 12) && enemyFilter.elapsedTime < 0.5f)
                .AddCanEnter(() =>
                {
                    // dont shoot into obstacles and allies
                    // additional effect: dont shoot into target exactly
                    return !Physics.Raycast(
                        origin:     transform.position, 
                        direction:  enemyFilter.ToTarget().ToPlane(),
                        maxDistance: enemyFilter.ToTarget().magnitude);
                })
                .AddOnBegin(() => tState.RestartRandom(1.0f, 1.5f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnEnd(tShoot.Restart)
                .AddOnUpdate(() =>
                {
                    inputHolder.keys[0] = true;
                    Vector2 desiredDirectionInput = (enemyFilter.GetTarget().position - transform.position).To2D();
                    inputHolder.directionInput = desiredDirectionInput;// Vector2.Lerp(inputHolder.directionInput, desiredDirectionInput, 0.1f);
                    //inputHolder.positionInput = desiredDirectionInput;
                })
                .AddShallReturn(tState.IsReady)
            ;

            Timer tDashCd = new Timer(0.5f);
            stateDashAway
                //.AddCanEnter(() => false)
                .SetUtility(() => 15f )//+ UtilityMathHelpers.ConditionalFactor(enemyFilter.IsCloser(4), 40) )
                .AddCanEnter(() => enemyFilter.IsCloser(7.5f))
                .AddOnBegin(() => tState.RestartRandom(0.5f, 0.5f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddCanEnter(tDashCd.IsReady)
                .AddOnEnd(tDashCd.Restart)
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

            var moveToDestination = new MoveToDestinationNavigation(seeker);
            stateMoveAway
                //.AddCanEnter(() => false)
                .SetUtility(() => 2f)
                .AddCanEnter(() => enemyFilter.IsCloser(6))
                .AddOnBegin(() => tState.RestartRandom(0.75f, 1.25f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnUpdate(() =>
                {
                    inputHolder.positionInput = moveToDestination.ToDestinationAdaptiveLerp(
                        enemyFilter.GetAwayFromTargetPosition(8) + Random.insideUnitCircle.To3D() * 3, 0.1f, 2.0f, 0.75f, 0.25f);
                })
                .AddShallReturn(() => moveToDestination.IsCloseToDestination(1))
                .AddShallReturn(tState.IsReady)
            ;

            var lookAt = new LookAt(transform);

            stateLookAt
                //.AddCanEnter(() => false)
                .AddOnBegin(inputHolder.ResetInput)
                .AddCanEnter(() => enemyFilter.IsFurther(6))
                .SetUtility(() => UtilityMathHelpers.ScallingUpTo(enemyFilter.GetTarget().elapsedTime, 0.5f, 3.0f))
                .AddOnBegin(() => lookAt.SetDestination(enemyFilter.GetTarget()))
                .AddOnUpdate(() => inputHolder.rotationInput = lookAt.UpdateRotationInput())
                .AddOnBegin(() => tState.RestartRandom(0.3f, 0.75f))
                .AddShallReturn(tState.IsReady)
            ;

            stateMoveBehind
                //.AddCanEnter(() => false)
                .AddCanEnter(() => !enemyFilter.IsAtBehindTargetPosition(1.5f))
                .SetUtility(() => 2)
                .AddOnBegin(() => tState.RestartRandom(0.75f,1.25f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnUpdate(() => DebugUtilities.DrawCross(enemyFilter.BehindTargetPosition(), 2, Color.red))
                .AddOnUpdate(() =>
                {
                    Vector3 toTarget = enemyFilter.StayBehindTargetAdaptive(1.0f, 600, 1f);
                    Vector3 target = toTarget + transform.position;

                    DebugUtilities.DrawCross(target, 2, Color.blue);

                    //inputHolder.positionInput = enemyFilter.StayBehindTargetAdaptive2D(1.0f, 400, 0.75f);
                    inputHolder.positionInput = moveToDestination.ToDestinationAdaptiveLerp(target, 0.1f, 1.0f, 0.75f, 2.25f);
                })
                //.AddShallReturn(() => moveToDestination.IsCloseToDestination(1))
                .AddShallReturn(tState.IsReady)
            ;

            /*var moveToDestination = new MoveToDestinationNavigation(seeker);

            stateMoveTo
                //.AddCanEnter(() => false)
                .SetUtility(() => 3.25f)
                .AddOnBegin(() => tState.RestartRandom(0.5f, 0.75f))
                .AddOnBegin(inputHolder.ResetInput)
                .AddOnUpdate(() =>
                {
                    inputHolder.positionInput = moveToDestination.ToDestinationAdaptiveLerp(
                        enemyFilter.GetTarget().position, 0.1f, 2.0f, 0.75f, 0.25f);
                })
                .AddShallReturn(tState.IsReady)
            ;

            stateMoveSide
                .AddCanEnter(() => false)
                .SetUtility(() => 0.5f)
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

            

            

            

            stateDashTowards
                .AddCanEnter(() => false)
                .SetUtility(() => 2.6f)
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

            /**/
        }
    }
}
