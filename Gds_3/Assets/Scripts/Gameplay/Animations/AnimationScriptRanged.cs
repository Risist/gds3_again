using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Ranged", menuName = "Ris/Animation/Script/Ranged")]
public class AnimationScriptRanged : AnimationScript
{
    [System.Serializable]
    public class ShootParams
    {
        [Range(0, 1)] public float shootTime;
        public float cd;

        [Header("Movement")]
        public RangedFloat forceAplicationPeriod = new RangedFloat(0, 0.2f);
        public float force = 100.0f;

        [Header("Rotation")]
        public RangedFloat rotationApplicationPeriod = new RangedFloat(0, 0.9f);
        public float rotationLerpScale = 0.15f;
        public float trackScale = 0.2f;

    }
    public ShootParams shootParams;

    [System.Serializable]
    public class DashParams
    {
        public float cd;

        [Header("Movement")]
        public RangedFloat forceAplicationPeriod = new RangedFloat(0, 0.2f);
        public float force = 100.0f;
    }
    public DashParams dashParams;

    public override void InitAnimation_Implementation()
    {
        var input = stateMachine.GetComponentInParent<InputHolder>();
        var bulletThrower = stateMachine.GetComponentInParent<BulletThrower>();
        //var headStorage = input.GetComponentInChildren<HeadDetectionStorage>();
        //var headPush = input.GetComponentInChildren<HeadPush>(true);
        var rigidbody = stateMachine.GetComponentInParent<Rigidbody>();

        var idle = stateMachine.AddNewStateAsCurrent("Idle");   // 0
        var shoot = stateMachine.AddNewState("Shoot");          // 1
        var dash = stateMachine.AddNewState("Dash");            // 2

        idle
            .AddUpdate((t) => input.rotationInput = input.directionInput)

            .AddTransition(shoot, new AnimationBlendData(0), false)
            .AddTransition(dash, new AnimationBlendData(0), false)
            ;



        Timer cdSlash = new Timer(shootParams.cd);
        {
            var motor = stateMachine.AddComponent(new AState_Motor(
                      Vector2.up * shootParams.force,
                      shootParams.forceAplicationPeriod));

            shoot.AddOnBegin(() => motor.Begin(input.atMove ? input.positionInput : input.transform.forward.To2D()));
            shoot.AddFixedUpdate(motor.FixedUpdate);

            stateMachine.AddComponent(shoot, new AState_RotationToDirection(
                shootParams.rotationApplicationPeriod,
                shootParams.rotationLerpScale,
                shootParams.trackScale));
        }


        BoxValue<bool> alreadyShoot = new BoxValue<bool>(false);
        shoot
            .AddIsPressed(() => input.keys[0])
            .AddCanEnter(() => cdSlash.IsReady())
            .AddOnBegin(() => alreadyShoot.value = false)
            .AddUpdate((s) =>
            {
                if (s > shootParams.shootTime && !alreadyShoot.value)
                {
                    bulletThrower.ThrowBullet();
                    alreadyShoot.value = true;
                }
            })
            .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))
            .AddOnEnd(cdSlash.Restart)
        ;


        {
            var motor = stateMachine.AddComponent(new AState_Motor(
                  Vector2.up * dashParams.force,
                  dashParams.forceAplicationPeriod));

            dash.AddOnBegin(() => motor.Begin(input.atMove ? input.positionInput : input.transform.forward.To2D()));
            dash.AddFixedUpdate(motor.FixedUpdate);
        }

        Timer cdDash = new Timer(dashParams.cd);
        dash
            .AddIsPressed(() => input.keys[1])
            .AddCanEnter(() => cdDash.IsReady())
            .AddOnEnd(cdDash.Restart)
            .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))
        ;
    }
}
