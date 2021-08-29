using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Humanoid", menuName = "Ris/Animation/Script/Humanoid")]
public class AnimationScriptHumanoid : AnimationScript
{
    [System.Serializable]
    public class SlashParams
    {
        public enum ESlashMode
        {
            EToPositionDirection,
            EToMouseDirection,
            EToPerpendicularDirection,
        }
        public ESlashMode slashMode;
        public float cd;

        //Implementujê to, poniewa¿ nie mamy rozdzielonych skryptów dla gracza i przeciwnika. Byæ mo¿e potem to zrobimy.
        public bool IsNPC = false;
        // 

        [Header("Rotation")]
        public RangedFloat rotationApplicationPeriod = new RangedFloat(0, 0.9f);
        public float rotationLerpScale = 0.15f;
        public float trackScale = 0.2f;


        [Header("Combo")]
        public RangedFloat comboTransitionPeriod = new RangedFloat(0.75f, 1.0f);
        public AnimationBlendData comboTransitionBlendData = new AnimationBlendData(0.25f);
        [Space]
        public RangedFloat dashComboTransitionPeriod = new RangedFloat(0.75f, 1.0f);
        public AnimationBlendData dashComboTransitionBlendData = new AnimationBlendData(0.25f);


        [Header("Movement")]
        public RangedFloat forceAplicationPeriod = new RangedFloat(0, 0.2f);
        public float force = 100.0f;
        public int defaultMovementDirection = -1;
        public float[] selectiveForce = new float[4] { 100.0f, 100.0f, 100.0f, 100.0f };
    }
    public SlashParams slashParams;


    [System.Serializable]
    public class DashParams
    {
        public enum EDashMode
        {
            EToPositionDirection,
            EToMouseDirection,
            EToPerpendicularDirection,
        }
        public EDashMode dashMode;
        public float cd;
        
        [Header("Rotation")]
        public RangedFloat rotationApplicationPeriod = new RangedFloat(0, 0.9f);
        public float rotationLerpScale = 0.15f;
        public float trackScale = 0.2f;

        [Header("Movement")]
        public RangedFloat forceAplicationPeriod = new RangedFloat(0, 0.2f);
        public float force = 100.0f;
        public int defaultMovementDirection = -1;
        public float[] selectiveForce = new float[4] { 100.0f, 100.0f , 100.0f , 100.0f };
        
    }

    public DashParams dashParams;

    public bool rotateToMouse = false;
    public string runningKey = "Running";

    public override void InitAnimation_Implementation()
    {
        var input = stateMachine.GetComponentInParent<InputHolder>();
        //var headStorage = input.GetComponentInChildren<HeadDetectionStorage>();
        //var headPush = input.GetComponentInChildren<HeadPush>(true);
        var rigidbody = stateMachine.GetComponentInParent<Rigidbody>();

        var idle = stateMachine.AddNewStateAsCurrent("Idle");   // 0
        var slash = stateMachine.AddNewState("Slash");          // 1
        var slashRunning = stateMachine.AddNewState("SlashRunning");          // 1
        var dash = stateMachine.AddNewState("Dash");            // 2
        //var whack = stateMachine.AddNewState("Whack");          // 3
        //var push = stateMachine.AddNewState("Push");            // 2




        idle
            .AddUpdate((t) => stateMachine.animator.SetBool(runningKey, input.atMove) )
            //.AddUpdate((t) => input.rotationInput = input.directionInput)
            //.AddTransition(push, new AnimationBlendData(0), false)
            .AddTransition(slashRunning, new AnimationBlendData(0), false)
            .AddTransition(slash, new AnimationBlendData(0), false)
            .AddTransition(dash, new AnimationBlendData(0.1f), false)
            //.AddTransition(whack, new AnimationBlendData(0), false)
            ;

        Timer cdSlash = new Timer(slashParams.cd);
        {
            var motor = stateMachine.AddComponent(new AState_Motor(
                  Vector2.up * slashParams.force,
                  slashParams.forceAplicationPeriod));

            slash.AddOnBegin(() => motor.Begin(input.atMove ? input.positionInput : input.transform.forward.To2D()));
            slash.AddFixedUpdate(motor.FixedUpdate);
        }

        slash
            .AddIsPressed(() => input.keys[0])
            .AddCanEnter(() => cdSlash.IsReady())
            .AddCanEnter(() => !input.atMove)
            .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))
            .AddOnEnd(cdSlash.Restart)
            //.AddTransition(push, new RangedFloat(0.7f, 1.0f), new AnimationBlendData(0.25f), false)
            //.AddTransition(slash, slashParams.comboTransitionPeriod, slashParams.comboTransitionBlendData, true)
        ;

        {
            var motor = stateMachine.AddComponent(new AState_Motor(
                  Vector2.up * slashParams.force,
                  slashParams.forceAplicationPeriod));

            slash.AddOnBegin(() => motor.Begin(input.atMove ? input.positionInput : input.transform.forward.To2D()));
            slash.AddFixedUpdate(motor.FixedUpdate);
        }

        slashRunning
            .AddIsPressed(() => input.keys[0])
            .AddCanEnter(() => cdSlash.IsReady())
            .AddCanEnter(() => input.atMove)
            .AddUpdate((s) => AutoTransition(idle, new AnimationBlendData(0)))
            .AddOnEnd(cdSlash.Restart)
            //.AddTransition(push, new RangedFloat(0.7f, 1.0f), new AnimationBlendData(0.25f), false)
            //.AddTransition(slash, slashParams.comboTransitionPeriod, slashParams.comboTransitionBlendData, true)
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

            //.AddTransition(slash, slashParams.dashComboTransitionPeriod, slashParams.comboTransitionBlendData)
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
