using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*
 * Specialized state machine for animation purposes
 * Designed to work well with standard unity animator
 *
 * Workflow:
 * Create AnimationController and put here required animations
 * Names of their nodes are important, you will reference them later
 * AnimationScript is responsible for creation of concrete animation qraph
 *
 * AnimationStates with param indicating name of node in unity's AnimationController
 * then define transitions, transitions and AnimationStates have their requirements,
 * first fitting transition is selected as next transition
 *
 * you have some lambda events, all can be initialized in cascade form
 */
//[RequireComponent(typeof(Animator))]
public class AnimationStateMachine : MonoBehaviour
{
    public Animator animator { get; private set; }
    public int stateMachineLayer = 0;
    public AnimationScript animationScript;
    public bool logState;

    public float animationTime { get => animator.GetCurrentAnimatorStateInfo(stateMachineLayer).normalizedTime; }
    public bool inTransition { get => animator.IsInTransition(stateMachineLayer); }
    public bool hasFinishedAnimation { get => animator.GetCurrentAnimatorStateInfo(stateMachineLayer).normalizedTime >= 1; }

    #region States
    // list of states entity can be in
    readonly List<AnimationState> _states = new List<AnimationState>();
    public AnimationState currentState { get; private set;}

    public bool AnimatorHasState(string animationName)
    {
        return AnimatorHasState(Animator.StringToHash(animationName));
    }
    public bool AnimatorHasState(int animationHash)
    {
        return animator.HasState(stateMachineLayer, animationHash);
    }

    public AnimationState GetState(int id = 0)
    {
        return _states[id];
    }

    public AnimationState AddNewState(string animationName)
    {
        if (!animator.HasState(stateMachineLayer, Animator.StringToHash(animationName)))
        {
            // no such state 
            return null;
        }

        AnimationState state = new AnimationState(animationName);
        _states.Add(state);
        return state;
    }
    public AnimationState AddNewStateAsCurrent(string animationName)
    {
        if (!AnimatorHasState(animationName))
        {
            // no such state 
            return null;
        }

        AnimationState state = new AnimationState(animationName);
        _states.Add(state);
        SetCurrentState(state);
        return state;
    }

    public void SetCurrentState(AnimationState target, AnimationBlendData blendData)
    {
        Debug.Assert(target != null);

        if (currentState != null)
            currentState.End();

        currentState = target;
        currentState.ResetBuffer(); 
        currentState.Begin();

        animator.CrossFade(target.animationHash, blendData.transitionDuration, stateMachineLayer, blendData.normalizedOffset);
    }
    public void SetCurrentState(AnimationState target)
    {
        Debug.Assert(target != null);
        if (currentState != null)
            currentState.End();

        currentState = target;
        currentState.ResetBuffer();
        currentState.Begin();

        animator.Play(target.animationHash);
    }


    public void ClearStates()
    {
        SetCurrentState(null);
        _states.Clear();
    }
    public void IterateOverStates(Action<AnimationState> action)
    {
        foreach (var it in _states)
            action(it);
    }
    public void ResetInputBuffer()
    {
        foreach (var it in _states)
            it.ResetBuffer();
    }
    #endregion States

    #region State Components
    public T AddComponent<T>(AnimationState state, T component) where T: AnimationStateComponent
    {
        component.stateMachine = this;
        component.OnComponentInit();
        component.OnComponentAdd(state);
        return component;
    }
    public T AddComponent<T>(T component) where T : AnimationStateComponent
    {
        component.stateMachine = this;
        component.OnComponentInit();
        return component;
    }

    #endregion State Components

    #region 
    public void Start()
    {
        animator = GetComponentInParent<Animator>();
        if(animationScript)
        {
            animationScript = Instantiate(animationScript);
            animationScript.InitAnimation(this);
        }
    }
    public void Update()
    {
        float animationTime = this.animationTime;
        if(logState)
        {
            Debug.Log("currentStateId: " + currentState);
        }

        Debug.Assert(currentState != null, "AnimationStateMachine: currentState has not been initialized");
        currentState.Update(animationTime);
        
        // transition only when all current transitions are finished
        if (!inTransition)
        {
            var transitionData = currentState.GetTransition(animationTime);
            if (transitionData == null)
                return;

            SetCurrentState(transitionData.target, transitionData.blendData);
        }
    }
    public void FixedUpdate()
    {
        float animationTime = this.animationTime;

        Debug.Assert(currentState != null);
        currentState.FixedUpdate(animationTime);
    }
    #endregion
}

public class AnimationTransition
{
    public AnimationState target;
    /// transition can only occur in specified part of animation
    public RangedFloat transitionRange;
    public bool bufferInput;

    /// additional requirements to use this transition
    public Func<bool> canEnter = () => true;

    public AnimationBlendData blendData;
}

[Serializable]
public struct AnimationBlendData
{
    public AnimationBlendData(float transitionDuration, float normalizedOffset = 0f)
    {
        this.transitionDuration = transitionDuration;
        this.normalizedOffset = normalizedOffset;
    }
    public float transitionDuration;
    public float normalizedOffset;
}

public class AnimationState
{
    public AnimationState(string animationName)
    {
        this.animationName = animationName;

#if UNITY_EDITOR
        animationNameString = animationName;
#endif
    }

    #region Animation
    public override string ToString()
    {
#if UNITY_EDITOR
        return animationNameString;
#else
        // for debug purposes
        // animation state is closely related to animation from animator
        // states will be identified by their hash
        return animationHash.ToString();
#endif
    }
    public int animationHash;
    public string animationName { set { animationHash = Animator.StringToHash(value); } }
#if UNITY_EDITOR
    public string animationNameString;
#endif
    #endregion Animation

    /// for more intuitive usage transitions will not require holding still action key
    /// if requirements are met for transition but it's too early for animation to transition
    ///     will for transition will be saved
    public bool bufferedInput { get; private set; }
    public void ResetBuffer() { bufferedInput = false; }


    #region Callbacks
    Action _onBegin = () => { };
    Action _onEnd = () => { };
    Action<float> _onUpdate = (animationTime) => { };
    Action<float> _onFixedUpdate = (animationTime) => { };
    readonly List<Func<bool>> _canEnter = new List<Func<bool>>();
    readonly List<Func<bool>> _isPressed = new List<Func<bool>>();

    public AnimationState AddOnBegin(Action s)
    {
        _onBegin += s;
        return this;
    }
    public AnimationState AddOnEnd(Action s)
    {
        _onEnd += s;
        return this;
    }

    public AnimationState AddUpdate(Action<float> s)
    {
        _onUpdate += s;
        return this;
    }
    public AnimationState AddFixedUpdate(Action<float> s)
    {
        _onFixedUpdate += s;
        return this;
    }
    public AnimationState AddCanEnter(Func<bool> s)
    {
        _canEnter.Add(s);
        return this;
    }
    public AnimationState AddIsPressed(Func<bool> s)
    {
        _isPressed.Add(s);
        return this;
    }

    public void Begin()
    {
        _onBegin();
    }
    public void End()
    {
        _onEnd();
    }
    public void Update(float animationTime)
    {
        _onUpdate(animationTime);
    }
    public void FixedUpdate(float animationTime)
    {
        _onFixedUpdate(animationTime);
    }
    #endregion Callbacks


    #region Transitions
    readonly List<AnimationTransition> _transitions = new List<AnimationTransition>();

    public AnimationState AddTransition(AnimationState target, bool bufferInput = true, Func<bool> canEnter = null)
    {
        AnimationTransition transition = new AnimationTransition
        {
            target = target, 
            transitionRange = new RangedFloat(),
            bufferInput = bufferInput
        };
        if(canEnter != null)
            transition.canEnter = canEnter;

        _transitions.Add(transition);
        return this;
    }
    public AnimationState AddTransition(AnimationState target, RangedFloat transitionRange, bool bufferInput = true, Func<bool> canEnter = null)
    {
        AnimationTransition transition = new AnimationTransition
        {
            target = target, 
            transitionRange = transitionRange, 
            bufferInput = bufferInput
        };
        if (canEnter != null)
            transition.canEnter = canEnter;

        _transitions.Add(transition);
        return this;
    }
    public AnimationState AddTransition(AnimationState target, AnimationBlendData transitionData, bool bufferInput = true, Func<bool> canEnter = null)
    {
        AnimationTransition transition = new AnimationTransition
        {
            target = target,
            transitionRange = new RangedFloat(),
            blendData = transitionData,
            bufferInput = bufferInput
        };
        if (canEnter != null)
            transition.canEnter = canEnter;

        _transitions.Add(transition);
        return this;
    }
    public AnimationState AddTransition(AnimationState target, RangedFloat transitionRange, AnimationBlendData transitionData, bool bufferInput = true, Func<bool> canEnter = null)
    {
        AnimationTransition transition = new AnimationTransition
        {
            target = target,
            transitionRange = transitionRange,
            blendData = transitionData,
            bufferInput = bufferInput
        };
        if (canEnter != null)
            transition.canEnter = canEnter;

        _transitions.Add(transition);
        return this;
    }


    public bool CanEnter()
    {
        foreach(var it in _canEnter)
            if( !it() )
                return false;
        return true;
    }
    public bool IsPressed()
    {
        foreach (var it in _isPressed)
            if (!it())
                return false;
        return true;
    }
    public AnimationTransition GetTransition(float animationTime)
    {
        // sequential transition prioritetization
        // transitions at front of the list are more preffered
        // other ones will pop up only if for any reason it was not siutable to use previous 
        foreach (var it in _transitions)
        {
            if (!it.canEnter())
                continue;

            var target = it.target;
            var p = it.transitionRange;
            bool pInRange = p.InRange(animationTime);
            if (!pInRange)
                return null;

            // buffer input if there was will to be in this state
            // and transition uses input buffer
            target.bufferedInput = target.IsPressed() 
                            || (it.bufferInput && target.bufferedInput);

            // use transition if input was buffered since last state change
            // and it is allowed to be in this state atm
            if (target.bufferedInput && target.CanEnter())
                return it;
        }

        // null means there was no transition, continue whatever you've been doing
        return null;
    }
    #endregion Transitions

}
