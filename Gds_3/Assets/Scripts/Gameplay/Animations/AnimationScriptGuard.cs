using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Guard", menuName = "Ris/Animation/Script/Guard")]
public class AnimationScriptGuard : AnimationScript
{
    [System.Serializable]
    public class SlashParams
    {
        public float cd;

        [Header("Combo")]
        public RangedFloat dashComboTransitionPeriod = new RangedFloat(0.75f, 1.0f);
        public AnimationBlendData dashComboTransitionBlendData = new AnimationBlendData(0.25f);


        [Header("Movement")]
        public RangedFloat forceAplicationPeriod = new RangedFloat(0, 0.2f);
        public float force = 100.0f;
    }
    public SlashParams slashParams;
    public SlashParams chargeParams;


    [System.Serializable]
    public class DashParams
    {
        public float cd;

        [Header("Movement")]
        public RangedFloat forceAplicationPeriod = new RangedFloat(0, 0.2f);
        public float force = 100.0f;

    }

    public DashParams dashParams;

    [System.Serializable]
    public class WhackParams
    {
        public float cd;
    }
    public WhackParams whackParams;

    public string runningKey = "Running";

    public override void InitAnimation_Implementation()
    {
        var input = stateMachine.GetComponentInParent<InputHolder>();
        var rigidbody = stateMachine.GetComponentInParent<Rigidbody>();

        var idle = stateMachine.AddNewStateAsCurrent("Idle");               // 0
        var slash = stateMachine.AddNewState("Slash");                      // 1
        var dash = stateMachine.AddNewState("Dash");                        // 2
        var whack = stateMachine.AddNewState("Whack");                      // 3
        var charge = stateMachine.AddNewState("SlashRunning");              // 4
        var slashRunning = stateMachine.AddNewState("SlashRunning");        // 5

        idle
            .AddUpdate((t) => stateMachine.animator.SetBool(runningKey, input.atMove))
            .AddUpdate((t) => input.rotationInput = input.directionInput)

            .AddTransition(slash,   new AnimationBlendData(0), false)
            .AddTransition(slashRunning, new AnimationBlendData(0), false)
            .AddTransition(charge, new AnimationBlendData(0), false)
            .AddTransition(dash,    new AnimationBlendData(0), false)
            .AddTransition(whack,   new AnimationBlendData(0), false)
            ;


        MinimalTimer cdAttack = new MinimalTimer();
        Timer cdDash = new Timer(dashParams.cd);

        {
            var motorSlash = stateMachine.AddComponent(new AState_Motor(
                  Vector2.up * slashParams.force,
                  slashParams.forceAplicationPeriod));

            slash
                //.AddCanEnter(() => false)
                .AddIsPressed(() => input.keys[0])
                .AddCanEnter(() => cdAttack.IsReady(slashParams.cd))
                .AddCanEnter(() => !input.atMove)

                .AddOnBegin(() => motorSlash.Begin(input.transform.forward.To2D()))
                
                .AddFixedUpdate(motorSlash.FixedUpdate)
                .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))
                
                .AddOnEnd(cdAttack.Restart)
            ;


            var motorSlashRunning = stateMachine.AddComponent(new AState_Motor(
                  Vector2.up * slashParams.force,
                  slashParams.forceAplicationPeriod));

            slashRunning
                //.AddCanEnter(() => false)
                .AddIsPressed(() => input.keys[0])
                .AddCanEnter(() => cdAttack.IsReady(slashParams.cd))
                .AddCanEnter(() => input.atMove)

                .AddOnBegin(() => motorSlash.Begin(input.positionInput))

                .AddFixedUpdate(motorSlashRunning.FixedUpdate)
                .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))

                .AddOnEnd(cdAttack.Restart)
            ;
        }

        {
            var motorCharge = stateMachine.AddComponent(new AState_Motor(
                  Vector2.up * chargeParams.force,
                  chargeParams.forceAplicationPeriod));

            charge
                //.AddCanEnter(() => false)
                .AddIsPressed(() => input.keys[4])
                .AddCanEnter(() => cdAttack.IsReady(chargeParams.cd))

                .AddOnBegin(() => motorCharge.Begin(input.atDirection ? input.directionInput : input.transform.forward.To2D()))

                .AddFixedUpdate(motorCharge.FixedUpdate)
                .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))

                .AddOnEnd(cdAttack.Restart)
            ;
        }




        {
            var motorDash = stateMachine.AddComponent(new AState_Motor(
                  Vector2.up * dashParams.force,
                  dashParams.forceAplicationPeriod));

            dash
                //.AddCanEnter(() => false)
                .AddIsPressed(() => input.keys[1])
                .AddCanEnter(() => cdDash.IsReady())

                .AddOnBegin(() => motorDash.Begin(input.atMove ? input.positionInput : input.transform.forward.To2D()))

                .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))
                .AddFixedUpdate(motorDash.FixedUpdate)

                .AddOnEnd(cdDash.Restart)

                .AddTransition(slash, slashParams.dashComboTransitionPeriod, slashParams.dashComboTransitionBlendData)
                .AddTransition(charge, chargeParams.dashComboTransitionPeriod, chargeParams.dashComboTransitionBlendData)
            ;
        }


        {
            whack
                //.AddCanEnter(() => false)
                .AddIsPressed(() => input.keys[3])
                .AddCanEnter(() => cdAttack.IsReady(whackParams.cd))

                .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))

                .AddOnEnd(cdAttack.Restart)
            ;
        }
    }
}

