using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    public abstract class BehaviourPack : ScriptableObject
    {
        public void DefineBehaviours(BehaviourController controller)
        {
            this.controller = controller;
            stimuliSettings = controller.GetComponentInChildren<MonoStimuliSettings>();
            DefineBehaviours_Impl();
        }
        protected abstract void DefineBehaviours_Impl();

        protected BehaviourController controller;
        protected MonoStimuliSettings stimuliSettings;
        protected StateMachine stateMachine => controller.stateMachine;
        protected GenericBlackboard blackboard => controller.blackboard;
        protected Transform transform => controller.transform;

        ////////// Utility
        /// sense type id's to reference blackboard content

        protected StimuliFilter GetEnemyFilter()    => stimuliSettings.GetEnemyFilter();
        protected StimuliFilter GetAllyFilter()     => stimuliSettings.GetAllyFilter();
        protected StimuliFilter GetNeutralFilter()  => stimuliSettings.GetNeutralFilter();
        protected StimuliFilter GetNoiseFilter()    => stimuliSettings.GetNoiseFilter();
        protected StimuliFilter GetPainFilter()     => stimuliSettings.GetPainFilter();
        protected StimuliFilter GetTouchFilter()    => stimuliSettings.GetTouchFilter();

        public void Interrupt(State to, System.Func<bool> condition = null)
        {
            if( stateMachine.CurrentState != to 
             && to.CanEnter()
             && (condition == null || condition()) 
             && stateMachine.CurrentState.CanBeInterrupted())
            {
                stateMachine.ChangeState(to);
            }
        }
    }
}
