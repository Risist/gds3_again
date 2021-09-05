using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Player", menuName = "Ris/Animation/Script/Player")]
public class AnimationScriptPlayer : AnimationScript
{
    [System.Serializable]
    public class SlashParams
    {
        public float cd;

        [Range(0.0f, 1.0f)] public float slashAnimationCancellingAfterDecapitation = 0.5f;

        [Header("Combo")]
        public RangedFloat comboTransitionPeriod = new RangedFloat(0.75f, 1.0f);
        public AnimationBlendData comboTransitionBlendData = new AnimationBlendData(0.25f);
        [Space]
        public RangedFloat dashComboTransitionPeriod = new RangedFloat(0.75f, 1.0f);
        public AnimationBlendData dashComboTransitionBlendData = new AnimationBlendData(0.25f);

        [Header("Rotation")]
        public RangedFloat rotationApplicationPeriod = new RangedFloat(0, 0.9f);
        public float rotationLerpScale = 0.15f;
        public float trackScale = 0.2f;

        [Header("Movement")]
        public RangedFloat forceAplicationPeriod = new RangedFloat(0, 0.2f);
        public float force = 100.0f;
    }
    public SlashParams slashParams;

    [System.Serializable]
    public class DashParams
    {
        public float cd;

        [Header("Movement")]
        public RangedFloat forceAplicationPeriod = new RangedFloat(0, 0.2f);
        public float force = 100.0f;

        [Header("Rotation")]
        public RangedFloat rotationApplicationPeriod = new RangedFloat(0, 0.9f);
        public float rotationLerpScale = 0.15f;
        public float trackScale = 0.2f;

    }
    public DashParams dashParams;

    public string runningKey = "Running";



    public override void InitAnimation_Implementation()
    {
        var input = stateMachine.GetComponentInParent<InputHolder>();
        var rigidbody = stateMachine.GetComponentInParent<Rigidbody>();

        var idle = stateMachine.AddNewStateAsCurrent("Idle");                   // 0
        var slash = stateMachine.AddNewState("Slash");                          // 1
        var slashRunning = stateMachine.AddNewState("SlashRunning");            // 2
        var dash = stateMachine.AddNewState("Dash");                            // 3

        #region animationCancel
        //
        bool flip = true;
        float calculateAnimationCancel()
        {
            if (flip)
            {
                flip = !flip;
                return slashParams.cd;
            }
            else
            {
                flip = !flip;
                return 1.0f;
            }
            
        }
        float animationCancelAmount = slashParams.slashAnimationCancellingAfterDecapitation;
        float animationCancel = 0.0f;
        //
        #endregion


        idle
            .AddUpdate((t) => stateMachine.animator.SetBool(runningKey, input.atMove))
            .AddUpdate((t) => input.rotationInput = input.directionInput)
            .AddTransition(slashRunning, new AnimationBlendData(0), false)
            .AddTransition(slash, new AnimationBlendData(0), false)
            .AddTransition(dash, new AnimationBlendData(0.1f), false)
            ;

        Timer cdSlash = new Timer(slashParams.cd);
        var motorSlash = stateMachine.AddComponent(new AState_Motor(
                  Vector2.up * slashParams.force,
                  slashParams.forceAplicationPeriod));
        stateMachine.AddComponent(slash, new AState_RotationToDirection(
                slashParams.rotationApplicationPeriod,
                slashParams.rotationLerpScale,
                slashParams.trackScale));

        slash
            .AddIsPressed(() => input.keys[0])
            .AddCanEnter(() => cdSlash.IsReady())
            .AddCanEnter(() => !input.atMove)
            .AddOnBegin(() => 
            {
                animationCancel = calculateAnimationCancel();
            })
            .AddOnBegin(() => motorSlash.Begin(input.transform.forward.To2D()))
            .AddFixedUpdate(motorSlash.FixedUpdate)

            .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))
            
            .AddOnEnd(() => cdSlash.Restart())
            .AddTransition(slash, new RangedFloat(animationCancelAmount, 1.0f), new AnimationBlendData(0.25f) )
            .AddTransition(slashRunning, new RangedFloat(animationCancelAmount, 1.0f), new AnimationBlendData(0.25f))
            .AddTransition(dash, new RangedFloat(0.3f, 1.0f), new AnimationBlendData(0.01f))
        ;

        var motorSlashRunning = stateMachine.AddComponent(new AState_Motor(
                  Vector2.up * slashParams.force,
                  slashParams.forceAplicationPeriod));

        stateMachine.AddComponent(slashRunning, new AState_RotationToDirection(
                slashParams.rotationApplicationPeriod,
                slashParams.rotationLerpScale,
                slashParams.trackScale));

        slashRunning
            .AddIsPressed(() => input.keys[0])
            .AddCanEnter(() => cdSlash.IsReady())
            .AddCanEnter(() => input.atMove)
            .AddOnBegin(() =>
            {
                animationCancel = calculateAnimationCancel();
            })
            .AddOnBegin(() => motorSlashRunning.Begin(input.positionInput))
            .AddFixedUpdate(motorSlashRunning.FixedUpdate)
            .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))
            .AddOnEnd(() => cdSlash.Restart())
            
            .AddTransition(slash, new RangedFloat(animationCancelAmount, 1.0f), new AnimationBlendData(0.35f))
            .AddTransition(slashRunning, new RangedFloat(animationCancelAmount, 1.0f), new AnimationBlendData(0.35f))
            .AddTransition(dash, new RangedFloat(0.3f, 1.0f), new AnimationBlendData(0.01f))
        ;

        var motorDash = stateMachine.AddComponent(new AState_Motor(
                Vector2.up * dashParams.force,
                dashParams.forceAplicationPeriod));
        stateMachine.AddComponent(dash, new AState_RotationToDirection(
            dashParams.rotationApplicationPeriod,
            dashParams.rotationLerpScale,
            dashParams.trackScale));

        Timer cdDash = new Timer(dashParams.cd);
        dash
            .AddIsPressed(() => input.keys[1])
            .AddCanEnter(() => cdDash.IsReady())

            .AddOnBegin(() => motorDash.Begin(input.atMove ? input.positionInput : input.transform.forward.To2D()))
            .AddFixedUpdate(motorDash.FixedUpdate)
            .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))
            
            .AddOnEnd(cdDash.Restart)

            .AddTransition(slashRunning, slashParams.dashComboTransitionPeriod, slashParams.comboTransitionBlendData)
            .AddTransition(slash, slashParams.dashComboTransitionPeriod, slashParams.comboTransitionBlendData)
        ;

        /* whack?
             .AddIsPressed(() => input.keys[3])
             .AddCanEnter(() => cdSlash.IsReady())
             .AddCanEnter(() => slashParams.IsNPC)
             .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))
             .AddOnEnd(cdSlash.Restart)
             //.AddTransition(push, new RangedFloat(0.7f, 1.0f), new AnimationBlendData(0.25f), false)
             //.AddTransition(slash, slashParams.comboTransitionPeriod, slashParams.comboTransitionBlendData, true)
         ;

         /*stateMachine.AddComponent(push, new AState_RotationToDirection(new RangedFloat(0, 0.9f), 0.05f, 0.9f));
         push
             .AddCanEnter(() => headStorage.HasAny )
             .AddOnBegin(() =>
             {
                 headPush.heads = headStorage.heads.Clone();

                 foreach (var obj in headPush.heads)
                 {
                     var dmg = 
                         obj.GetComponent<DamageOnCollision>() ??
                         obj.gameObject.AddComponent<DamageOnCollision>();
                     dmg.damageDataContinous = new DamageData();
                     dmg.damageDataEnter = new DamageData();
                     dmg.damageDataOnce = new DamageData();
                     dmg.damageDataContinous.damage = 10000;
                     dmg.damageDataEnter.damage = 10000;
                 }
             })
             .AddIsPressed(() =>
             {
                 return input.keys[0];
             })
             .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))
         ;*/
    }

}
