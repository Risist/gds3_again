using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;
using NaughtyAttributes;


namespace Ai
{
    // TODO add option for debug draw text displaying currentState name
    // TODO add a coroutine which will allow to specify behaviour refresh rate
    //      ideas: split ai execution to several groups

    public class BehaviourController : MonoBehaviour
    {
        #region References

        [NonSerialized] public readonly StateMachine stateMachine = new StateMachine();

        #endregion References

        #region BehaviourPack

        [SerializeField, Tooltip("Behaviour packs are used to insert groups of behaviours to this agent")]
        BehaviourPack[] _behaviourPacks;

        BehaviourPack[] _instantiatedBehaviourPacks;

        void InitBehaviourPacks()
        {
            _instantiatedBehaviourPacks = new BehaviourPack[_behaviourPacks.Length];

            for (int i = 0; i < _behaviourPacks.Length; ++i)
            {
                _instantiatedBehaviourPacks[i] = Instantiate(_behaviourPacks[i]);
                _instantiatedBehaviourPacks[i].DefineBehaviours(this);
            }
        }

        #endregion BehaviourPack

        public readonly GenericBlackboard blackboard = new GenericBlackboard();

        #region Perception
        public readonly List<StimuliStorage> senseStorage = new List<StimuliStorage>();
        public void RegisterSense(StimuliStorage storage)
        {
            senseStorage.Add(storage);
        }

        #endregion



        private void Start()
        {
            InitBehaviourPacks();
            //Debug.Assert(stateMachine.CurrentState != null, "No behaviour pack initialized current state");
#if UNITY_EDITOR
            var visualLogger = GetComponentInParent<VisualLogger>();
            if (visualLogger)
            {
                currentStateData = visualLogger.AddLog(new Vector2(0, -100), Color.red, "", displayCurrentStateName);
                currentModeData = visualLogger.AddLog(new Vector2(0, -140), Color.blue, "", displayCurrentModeName);
            }
#endif
        }

        private void FixedUpdate()
        {
            stateMachine.UpdateStates();

#if UNITY_EDITOR
            currentStateData.enabled = displayCurrentStateName;
            
            if (stateMachine.CurrentState != null)
            {
                currentStateData.text = stateMachine.CurrentState.stateName;
            }

            var selector = GetComponentInChildren<MonoAttentionSelector>();
            currentModeData.enabled = selector && displayCurrentModeName;
            if (currentModeData.enabled)
            {
                currentModeData.text = selector?.attentionSelector?.CurrentMode?.ModeName;
            }
#endif
        }

#if UNITY_EDITOR
        [BoxGroup(AttributeHelper.HEADER_DEBUG)]
        public bool displayCurrentStateName = false;

        [BoxGroup(AttributeHelper.HEADER_DEBUG)]
        public bool displayCurrentModeName = false;

        VisualLogger.LogData currentStateData;
        VisualLogger.LogData currentModeData;
#endif
    }

}
