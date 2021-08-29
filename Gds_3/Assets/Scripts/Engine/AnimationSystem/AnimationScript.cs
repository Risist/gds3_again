using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AnimationScript : ScriptableObject
{
    protected AnimationStateMachine stateMachine { get; private set; }

    public void InitAnimation(AnimationStateMachine animationStateMachine)
    {
        this.stateMachine = animationStateMachine;
        InitAnimation_Implementation();
    }

    public abstract void InitAnimation_Implementation();

    protected  void AutoTransition(AnimationState targetState, float transitionDuration = 0.0f, float transitionOffset = 0.0f, float transitionTime = 1.0f)
    {
        float animationTime = stateMachine.animationTime;
        if(!stateMachine.inTransition && animationTime >= transitionTime)
        {
            var blendData = new AnimationBlendData(transitionDuration, transitionOffset);
            stateMachine.SetCurrentState(targetState, blendData);
        }
    }
    protected void AutoTransition( AnimationState targetState, AnimationBlendData blendData, float transitionTime = 1.0f)
    {
        float animationTime = stateMachine.animationTime;
        if (!stateMachine.inTransition && animationTime >= transitionTime)
        {
            stateMachine.SetCurrentState(targetState, blendData);
        }
    }

    protected void SetCd(AnimationState state, Timer cd)
    {
        state.AddCanEnter(cd.IsReady);
        state.AddOnEnd(cd.Restart);
    }


    protected void Motor(Rigidbody2D rb, Vector2 movementSpeed)
    {
        rb.AddForce(movementSpeed * rb.mass);
    }
    protected void Motor(Rigidbody2D rb, Vector2 movementSpeed, float animationTime, RangedFloat applicationPeriod)
    {
        if (applicationPeriod.InRange(animationTime))
            Motor(rb, movementSpeed);
    }

    protected void Motor(Rigidbody rb, Vector2 movementSpeed)
    {
        rb.AddForce(movementSpeed, ForceMode.Acceleration);
    }
    protected void Motor(Rigidbody rb, Vector2 movementSpeed, float animationTime, RangedFloat applicationPeriod)
    {
        if (applicationPeriod.InRange(animationTime))
            Motor(rb, movementSpeed);
    }
}
