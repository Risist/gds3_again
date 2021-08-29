using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

/*
namespace Ai
{
    public class AttentionMode : StateSet
    {
        public AttentionMode(FocusBase focus = null)
        {
            this.focus = focus;
        }

        #region Propertities
        // focus connected to this attention mode
        // note: can be equal null (for example for idle behaviours )
        public FocusBase focus { get; private set; }

        public bool hadTarget { get; private set; }

        MinimalTimer _modeActiveTimer = new MinimalTimer();
        public float activeTime { get { return _modeActiveTimer.ElapsedTime(); } }

        // TODO
        public bool isCurrentMode { get; private set; }
        #endregion Propertities

        #region callbacks
        // returns utility of the mode at the given time
        Func<float> _utilityMethod = () => 1.0f;
        // called when mode starts execution
        Action _modeEnterMethod = () => { };
        // called when mode ends it's execution
        Action _modeExitMethod = () => { };

        public AttentionMode SetUtility(Func<float> utilityMethod)
        {
            this._utilityMethod = utilityMethod;
            return this;
        }
        public AttentionMode AddModeEnterAction(Action action)
        {
            _modeEnterMethod += action;
            return this;
        }
        public AttentionMode AddModeExitAction(Action action)
        {
            _modeExitMethod += action;
            return this;
        }

        public float GetUtility()
        {
            return _utilityMethod();
        }
        public void OnModeEnter()
        {
            isCurrentMode = true;
            _modeActiveTimer.Restart();
            _modeEnterMethod();
        }
        public void OnModeExit()
        {
            isCurrentMode = false;
            _modeExitMethod();
        }
        #endregion callbacks

       

        #region Public functions
        public MemoryEvent GetTarget()
        {
            if (focus != null)
            {
                // restart modeTime
                //bool hasTarget = focus.HasTarget();
                //if (!hadTarget && hasTarget)
                //    _modeActiveTimer.Restart();
                //hadTarget = hasTarget;

                return focus.GetTarget();
            }
            return null;
        }

        public State AddNewState(bool abortImmidiatelly = true)
        {
            return AddNewState<State>(abortImmidiatelly);
        }
        public T AddExistingState<T>(T state, bool abortImmidiatelly = true) where T : State, new()
        {
            base.AddExistingState(state);

            // return state execution immidiately if mode has changed
            if (abortImmidiatelly)
                state.AddShallReturn(() => !isCurrentMode);
            return state;
        }
        public T AddNewState<T>(bool abortImmidiatelly = true) where T : State, new()
        {
            var state = new T();
            AddExistingState(state, abortImmidiatelly);
            return state;
        }
        #endregion Public functions
    }

    public class FocusPriority : FocusBase
    {
        public FocusPriority(Transform transform) : base(transform) {}

        // current AttentionMode has increased utility by this value
        // it is to reduce frequently changes to mode and through this to states
        public float sustainUtility = 0;
        public FocusPriority SetSustainUtility(float value)
        {
            sustainUtility = value;
            return this;
        }
        public FocusBase currentFocus { get => currentMode?.focus; }

        #region transitions
        // aliases of TransitionSet GetNextState functions
        // intended to be used inside states
        // 

        public State GetRandomTransition()
        {
            return currentMode.GetRandomTransition();
        }
        public State GetTransitionByRandomUtility()
        {
            return currentMode.GetTransitionByRandomUtility();
        }
        public State GetTransitionByBestUtility()
        {
            return currentMode.GetTransitionByBestUtility();
        }
        public State GetTransitionByOrder()
        {
            return currentMode.GetTransitionByOrder();
        }
        #endregion transitions

        #region Modes
        readonly List<AttentionMode> modes = new List<AttentionMode>();
        public AttentionMode currentMode { get; private set; }

        public AttentionMode AddAttentionMode(AttentionMode mode)
        {
            Debug.Assert(mode != null);
            modes.Add(mode);
            return mode;
        }

        // called when mode is changed to a new one
        Action _onModeChange = () => { };
        public FocusPriority AddOnModeChange(Action action)
        {
            _onModeChange += _onModeChange;
            return this;
        }
        #endregion Modes        

        #region update
        /// changes current mode if any other callback has greather utility value
        /// if mode has changed calls _onModeChange callback
        /// is called from AiBehaviourController before state machine update
        public void UpdateCurrentMode()
        {
            // find which mode is the best right now
            float bestModeUtility = currentMode != null ? currentMode.GetUtility() + sustainUtility : float.MinValue;
            AttentionMode bestMode = currentMode;
            int bestI = -1;
            for (int i = 0; i < modes.Count; ++i)
            // intentionally no if check agains currentMode => it's waste in performance
            {
                AttentionMode currentMode = modes[i];
                float utility = currentMode.GetUtility();
                if (utility > bestModeUtility)
                {
                    bestI = i;
                    bestModeUtility = utility;
                    bestMode = currentMode;
                }
            }

            if (currentMode != bestMode)
            {
                // if mode has changed invoke callback
                _onModeChange();

                currentMode?.OnModeExit();
                bestMode?.OnModeEnter();

                currentMode = bestMode;
                //Debug.Log(bestI);
            }
        }

        public override MemoryEvent GetTarget()
        {
            return currentMode?.GetTarget();
        }
        #endregion update
    }
}*/
