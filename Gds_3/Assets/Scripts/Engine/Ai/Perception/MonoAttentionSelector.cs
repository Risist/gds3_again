using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;
using System;

namespace Ai
{
    // AttentionMode : idle, enemy, noise, Herd,
    //  awareness level: seen, target lost, 
    // priority


    // TODO
    public enum EAwarenessLevel
    {
        EUnaware            = 0, // agent does not know of any information
        ESuspicious         = 1, // agent considers target event as very inacurrate, only information that something happened is possible for use
        EInaccurate         = 2, // agent considers target event as inacurrate
        ESeen               = 3, // agent considers target event as accurate data
    }




    public class MonoAttentionSelector : MonoBehaviour
    {
        public readonly AttentionSelector attentionSelector = new AttentionSelector();

        [Serializable]
        public class AttentionModeSettings
        {
            public string displayName = "";
            public bool enabled                         = true;
            public EStimuliFilterType filterType        = EStimuliFilterType.EEnemy;
            public EAwarenessLevel awarenessLevel       = EAwarenessLevel.ESeen;
            public RangedFloat activationPeriod         = new RangedFloat(0,1.5f);
            public float priority                       = 1.0f;

            [NonSerialized] public AttentionMode mode = new AttentionMode("");
        }

        public List<AttentionModeSettings> attentionModes = new List<AttentionModeSettings>();
        public AttentionMode GetAttentionMode(EStimuliFilterType filterType, EAwarenessLevel awarenessLevel)
        {
            if(filterType == EStimuliFilterType.EIdle)
            {
                return _idleMode;
            }

            foreach(var it in attentionModes)
            {
                if( it.filterType == filterType && it.awarenessLevel == awarenessLevel)
                {
                    return it.mode;
                }
            }

            Debug.LogWarning("Trying to obtain non existent attention mode");
            return null;
        }

        AttentionMode _idleMode = new AttentionMode("Idle");
        public AttentionMode idleMode => _idleMode;


        void InitAttentionMode(AttentionModeSettings settings, AttentionMode mode, StimuliFilter filter)
        {
            if (!settings.enabled)
                return;

#if UNITY_EDITOR
            mode.ModeName = settings.displayName;
#endif

            attentionSelector
                .AddAttentionMode(mode)
                .SetGetEventMethod(filter.GetTarget)
                .SetActivationPeriod(settings.activationPeriod)
                .SetPriority(settings.priority);
            ;
        }
        void InitIdleAttentionMode()
        {
            attentionSelector
                .SetIdleAttentionMode(idleMode)
                .SetPriority(float.MinValue);
        }

        public void InitializeAttentionSelector()
        {
            var stimuliSettings = GetComponentInChildren<MonoStimuliSettings>();

            foreach(var it in attentionModes)
            {
                InitAttentionMode(it, it.mode, stimuliSettings.GetFilter(it.filterType));
            }

            InitIdleAttentionMode();

            attentionSelector.Init(stimuliSettings.stateMachine);
        }

        IEnumerator Start()
        {
            yield return null;
            InitializeAttentionSelector();
        }


    }

    public class AttentionSelector
    {
        StateMachine stateMachine;


        #region Modes
        List<AttentionMode> _modeList = new List<AttentionMode>();
        AttentionMode _idleMode;

        public AttentionMode CurrentMode { get; private set; }

        // inserts new possible attention mode
        // addition order matters!
        // modes will be checked in the order they were inserted
        public AttentionMode AddAttentionMode(AttentionMode attentionMode)
        {
            _modeList.Add(attentionMode);


            return attentionMode;
        }

        // inserts default, MemoryEvent independent mode
        public AttentionMode SetIdleAttentionMode(AttentionMode attentionMode)
        {
            _idleMode = attentionMode;

            return attentionMode;
        }


        // forces attention mode to be changed
        // passed mode should not be a null
        public void ChangeCurrentMode(AttentionMode newMode)
        {
            Debug.Assert(newMode != null, "AttentionSelector::ChangeCurrentMode, trying to change to null mode");
            Debug.Assert(newMode != CurrentMode, "AttentionSelector::ChangeCurrentMode, trying to change current mode");

            _onModeChange(CurrentMode, newMode);

            if (CurrentMode != null)
            {
                CurrentMode.DeactivateMode();
            }

            CurrentMode = newMode;

            CurrentMode.ActivateMode(stateMachine);

            stateMachine.ChangeState(CurrentMode.GetNextState());
        }


        // runs action when state changes
        // method params:
        //  @currentMode -> mode we transition from
        //  @newMode -> mode we transition to
        Action<AttentionMode, AttentionMode> _onModeChange = (currentMode, newMode) => { };

        // runs action when state changes
        // method params:
        //  @currentMode -> mode we transition from
        //  @newMode -> mode we transition to
        public void AddOnModeChanged(Action<AttentionMode, AttentionMode> method)
        {
            if (method != null)
                _onModeChange += method;
        }

        #endregion 

        #region Live cycle
        // initializes default state of AttentionSelector
        // should be called after all initially available modes are set up
        public void Init(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;

            // Initialize _currentMode to initial value
            ChangeCurrentMode(_idleMode);

            // each state machine update calls update function to check if attention mode switch is needed
            stateMachine.AddOnUpdate((state) => Update());
            stateMachine.SetNextStateMethod(GetNextState);
        }

        State GetNextState()
        {
            return CurrentMode.GetNextState();
        }

        // updates state of attention
        void Update()
        {
            //UpdateAttentionState_Order();
            UpdateAttentionState_Priority();
        }

        void UpdateAttentionState_Order()
        {
            foreach (var it in _modeList)
            {
                if (it.CanBeActivated())
                {
                    if (it != CurrentMode)
                    {
                        ChangeCurrentMode(it);
                    }
                    return;
                }
            }

            if (CurrentMode != _idleMode)
            {
                ChangeCurrentMode(_idleMode);
            }
        }
        void UpdateAttentionState_Priority()
        {
            var bestMode = _idleMode;
            float bestPriority = float.MinValue;

            foreach (var it in _modeList)
            {
                if (it.CanBeActivated())
                {
                    float priority = it.GetPriority();
                    if(priority > bestPriority)
                    {
                        bestPriority = priority;
                        bestMode = it;
                    }
                }
            }

            if (CurrentMode != bestMode)
            {
                ChangeCurrentMode(bestMode);
            }
        }

        #endregion

    }

    [Serializable]
    public class AttentionMode : StateSet
    {
    #if UNITY_EDITOR
        public string ModeName;
        public override string ToString()
        {
            return ModeName;
        }
    #endif

        public AttentionMode(string ModeName)
        {

    #if UNITY_EDITOR
            this.ModeName = ModeName;
    #endif

            _getNextState = GetTransitionByRandomUtility;
        }

        // What activation time event has to have
        public RangedFloat activationPeriod;

        public AttentionMode SetActivationPeriod(RangedFloat newActivationPeriod)
        {
            activationPeriod = newActivationPeriod;
            return this;
        }



        #region Flow Controll
        public bool Activated { get; private set; }

        MinimalTimer _tModeActivated = new MinimalTimer();

        StateMachine _currentStateMachine;

        // time elapsed since Mode activation
        // returns valid information only when the mode is Activated
        public float ModeActivationTime => _tModeActivated.ElapsedTime();

        public bool CanBeActivated()
        {
            if(_getEvent == null)
            {
                return true;
            }

            var ev = _getEvent();
            return ev != null && ev.perceiveUnit != null && activationPeriod.InRange(ev.elapsedTime);
        }

        public void ActivateMode(StateMachine stateMachine)
        {
            Activated = true;
            _tModeActivated.Restart();

            _onEnter();
            _currentStateMachine = stateMachine;
            stateMachine.ChangeState(_getNextState());
            stateMachine.AddOnUpdate(OnUpdate);
        }
        public void DeactivateMode()
        {
            Activated = false;
            _onExit();
            _currentStateMachine.RemoveOnUpdate(OnUpdate);
        }

        void OnUpdate(State state)
        {
            _onUpdate();
        }
        #endregion 


        #region Callbacks

        Func<MemoryEvent> _getEvent = null;
        public AttentionMode SetGetEventMethod(Func<MemoryEvent> method)
        {
            _getEvent = method;
            return this;
        }

        // sets default transition method for states
        // default selection method can be overriden in state settings
        // if state method returns null (what happens by default) then this function is used instead
        Func<State> _getNextState;

        // sets default state transition method
        // this setting can be overriden by state
        // is state's method return null then default setting will be used
        public AttentionMode SetNextStateMethod(Func<State> method)
        {
            if (method != null)
                _getNextState = method;

            return this;
        }

        public State GetNextState()
        {
            return _getNextState();
        }

        #endregion

        #region Callbacks
        
        Action _onEnter = () => { };

        public AttentionMode AddOnEnter(Action onEnter)
        {
            _onEnter += onEnter;
            return this;
        }
        public AttentionMode RemoveOnEnter(Action onEnter)
        {
            _onEnter -= onEnter;
            return this;
        }

        Action _onUpdate = () => { };

        public AttentionMode AddOnUpdate(Action onUpdate)
        {
            _onUpdate += onUpdate;
            return this;
        }
        public AttentionMode RemoveOnUpdate(Action onUpdate)
        {
            _onUpdate -= onUpdate;
            return this;
        }


        Action _onExit = () => { };

        public AttentionMode AddOnExit(Action onExit)
        {
            _onExit += onExit;
            return this;
        }
        public AttentionMode RemoveOnExit(Action onExit)
        {
            _onExit -= onExit;
            return this;
        }

        Func<float> _priority = () => { return 1.0f; };
        public AttentionMode SetPriority(Func<float> priority)
        {
            if (priority != null)
                _priority = priority;

            return this;
        }
        public AttentionMode SetPriority(float priority)
        {
            _priority = () => priority;
            return this;
        }

        public float GetPriority()
        {
            return _priority();
        }

        #endregion

        #region State initialization

        bool State_ShallReturn()
        {
            return !CanBeActivated();
        }
        bool State_CanEnter()
        {
            return CanBeActivated();
        }

        public new T AddExistingState<T>(T state, string stateName = "NoName") where T : State, new()
        {
            base.AddExistingState<T>(state, stateName);

            state.AddCanEnter(State_CanEnter);
            state.AddShallReturn(State_ShallReturn);

            return state;
        }

        public new State AddNewState(string stateName = "NoName")
        {
            return AddNewState<State>(stateName);
        }

        public new T AddNewState<T>(string stateName = "NoName") where T : State, new()
        {
            var state = new T();
            AddExistingState(state, stateName);
            return state;
        }

        #endregion

    }

}