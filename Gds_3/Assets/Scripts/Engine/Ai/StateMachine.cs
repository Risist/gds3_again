using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

namespace Ai
{

    public class StateMachine : StateSet
    {
        public StateMachine()
        {
            _getNextState = GetTransitionByRandomUtility;
        }

        public State CurrentState { get; private set; }

        #region Callbacks

        // sets default transition method for states
        // default selection method can be overriden in state settings
        // if state method returns null (what happens by default) then this function is used instead
        Func<State> _getNextState;

        // sets default state transition method
        // this setting can be overriden by state
        // is state's method return null then default setting will be used
        public void SetNextStateMethod(Func<State> method)
        {
            if (method != null)
                _getNextState = method;
        }


        // runs action when state changes
        // method params:
        //  @currentState -> state we transition from
        //  @newState -> state we transition to
        Action<State, State> _onStateChange = (currentState, newState) => { };

        // runs action when state changes
        // method params:
        //  @currentState -> state we transition from
        //  @newState -> state we transition to
        public void AddOnStateChanged(Action<State, State> method)
        {
            if (method != null)
                _onStateChange += method;
        }
        public void RemoveOnStateChanged(Action<State, State> method)
        {
            if (method != null)
                _onStateChange -= method;
        }


        // runs action every state machine update
        // executes before state update
        // method params:
        //  @currentState -> currently executed behaviour
        Action<State> _onUpdate = (currentState) => { };

        // runs action every state machine update
        // executes before state update
        // method params:
        //  @currentState -> currently executed behaviour
        public void AddOnUpdate(Action<State> method)
        {
            if (method != null)
                _onUpdate += method;
        }
        public void RemoveOnUpdate(Action<State> method)
        {
            if (method != null)
                _onUpdate -= method;
        }

        #endregion Callbacks

        #region Init

        public State AddExistingStateAsCurrent(State state, string stateName = "NoName")
        {
            AddExistingState(state, stateName);
            ChangeState(state);
            return state;
        }

        public State AddNewStateAsCurrent(string stateName = "NoName")
        {
            var state = AddNewState(stateName);
            ChangeState(state);
            return state;
        }

        #endregion Init

        #region UpdateCycle

        State GetNextState()
        {
            if (CurrentState == null)
            {
                //Debug.LogWarning(nameof(CurrentState) + " is not initialized to any value");
                return null;
            }

            return CurrentState?.GetNextState() ?? _getNextState();
        }

        public void UpdateStates()
        {
            if (CurrentState == null || CurrentState.ShallReturn() )
            {
                var state = GetNextState();
                bool canTransitionToSelf = CurrentState != null && CurrentState.CanTransitionToSelf();
                bool b = state != CurrentState || canTransitionToSelf;
                if (state != null && b)
                {
                    ChangeState(GetNextState);
                }
                if(CurrentState == null)
                {
                    return;
                }
            }

            _onUpdate(CurrentState);
            CurrentState.Update();
        }

        public void ChangeState(State newState)
        {
            if (newState == null)
            {
                Debug.LogWarning("Changing state to null");
                return;
            }
            var oldState = CurrentState;

            CurrentState?.End();
            CurrentState = newState;
            CurrentState.Begin();

            _onStateChange(oldState, CurrentState);
        }
        public void ChangeState(Func<State> newState)
        {
            if (newState == null)
            {
                Debug.LogWarning("Changing state to null");
                return;
            }
            var oldState = CurrentState;

            CurrentState?.End();
            CurrentState = newState();
            CurrentState.Begin();

            _onStateChange(oldState, CurrentState);
        }

        #endregion
    }

    public class State
    {
#if UNITY_EDITOR
        public string stateName = "NoName";
#endif

        protected Action _onBegin = () => { };
        protected Action _onEnd = () => { };
        protected Action _onUpdate = () => { };
        protected Func<State> _getNextState = () => null;
        protected Func<float> _getUtility = () => 1.0f;
        protected readonly List<Func<bool>> _shallReturn = new List<Func<bool>>();
        protected readonly List<Func<bool>> _canEnter = new List<Func<bool>>();
        protected readonly List<Func<bool>> _canTransitionToSelf = new List<Func<bool>>();
        protected readonly List<Func<bool>> _canBeInterrupted = new List<Func<bool>>();

        #region Public Functions

        public bool CanTransitionToSelf()
        {
            foreach (var it in _canTransitionToSelf)
                if (!it())
                    return false;
            return true;
        }

        public bool CanEnter()
        {
            foreach (var it in _canEnter)
                if (!it())
                    return false;
            return true;
        }

        public bool ShallReturn()
        {
            foreach (var it in _shallReturn)
                if (it())
                    return true;
            return false;
        }

        public bool CanBeInterrupted()
        {
            foreach (var it in _canBeInterrupted)
                if (!it())
                    return false;
            return true;
        }

        public float GetUtility()
        {
            return _getUtility();
        }

        public State GetNextState()
        {
            return _getNextState();
        }

        public void Update()
        {
            _onUpdate();
        }

        public void Begin()
        {
            _onBegin();
        }

        public void End()
        {
            _onEnd();
        }

        #endregion Public Functions

        #region Init Functions

        public State AddOnBegin(System.Action s)
        {
            if (s != null)
                _onBegin += s;
            return this;
        }

        public State AddOnEnd(System.Action s)
        {
            if (s != null)
                _onEnd += s;
            return this;
        }

        public State AddOnUpdate(System.Action s)
        {
            if (s != null)
                _onUpdate += s;
            return this;
        }

        public State SetNextStateMethod(Func<State> s)
        {
            if (s != null)
                _getNextState = s;
            return this;
        }

        public State AddShallReturn(Func<bool> s)
        {
            if (s != null)
                _shallReturn.Add(s);
            return this;
        }

        public State AddCanBeInterrupted(Func<bool> s)
        {
            if (s != null)
                _canBeInterrupted.Add(s);
            return this;
        }

        public State SetUtility(Func<float> s)
        {
            if (s != null)
                _getUtility = s;
            return this;
        }

        public State SetUtility(float s)
        {
            _getUtility = () => s;
            return this;
        }

        public State AddCanEnter(Func<bool> s)
        {
            if (s != null)
                _canEnter.Add(s);
            return this;
        }

        public State AddCanTransitionToSelf(Func<bool> s)
        {
            if (s != null)
                _canTransitionToSelf.Add(s);
            return this;
        }

        #endregion Init Functions
    }


//predefined transitions
    public class StateSet
    {
        protected readonly List<State> _states = new List<State>();
        public int Count => _states.Count;

        public State GetState(int i) => _states[i];

        public int GetStateIndex(State state)
        {
            return _states.FindIndex((o) => o == state);
        }

        public State AddNewState(string stateName = "NoName")
        {
            return AddNewState<State>(stateName);
        }

        public T AddExistingState<T>(T state, string stateName = "NoName") where T : State, new()
        {
            _states.Add(state);
#if UNITY_EDITOR
            state.stateName = stateName;
#endif
            return state;
        }

        public T AddNewState<T>(string stateName = "NoName") where T : State, new()
        {
            var state = new T();
            AddExistingState(state, stateName);
            return state;
        }

        #region NextStateMethods

        public State GetRandomTransition()
        {
            return _states[Random.Range(0, _states.Count)];
        }

        public State GetTransitionByRandomUtility()
        {
            StaticCacheLists.floatCache.Clear();

            float sum = 0;
            var behaviours = _states;
            for (int i = 0; i < behaviours.Count; ++i)
            {
                StaticCacheLists.floatCache.Add(0);
                if (behaviours[i].CanEnter())
                {
                    StaticCacheLists.floatCache[i] = (Mathf.Clamp(behaviours[i].GetUtility(), 0, float.MaxValue));
                    sum += StaticCacheLists.floatCache[i];
                }
            }

            if (sum == 0)
                return null;

            float randed = UnityEngine.Random.Range(0, sum);

            float lastSum = 0;
            for (int i = 0; i < behaviours.Count; ++i)
            {
                float utility = StaticCacheLists.floatCache[i];
                if (behaviours[i].CanEnter())
                {
                    if (randed >= lastSum && randed <= lastSum + utility)
                    {
                        return behaviours[i];
                    }
                    else
                        lastSum += utility;
                }
            }

            return null;
        }

        public State GetTransitionByBestUtility()
        {
            // just a linear search
            float bestUtility = float.MinValue;
            State bestState = null;

            foreach (var it in _states)
            {
                if (!it.CanEnter())
                    continue;

                var utility = it.GetUtility();
                if (utility > bestUtility)
                {
                    bestState = it;
                    bestUtility = utility;
                }
            }

            return bestState;
        }

        public State GetTransitionByOrder()
        {
            foreach (var it in _states)
                if (it.CanEnter())
                    return it;
            return null;
        }

        #endregion NextStateMethods
    }
}