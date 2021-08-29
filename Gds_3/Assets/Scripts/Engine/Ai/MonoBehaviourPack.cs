using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ai
{
    // monobehaviour version of Behaviour pack
    // used when manually set references are needed
    public class MonoBehaviourPack : MonoBehaviour
    {
        protected void Start()
        {
            controller = GetComponentInParent<BehaviourController>();
            stimuliSettings = controller.GetComponentInChildren<MonoStimuliSettings>();
        }

        protected BehaviourController controller;
        protected MonoStimuliSettings stimuliSettings;
        protected StateMachine stateMachine => controller.stateMachine;
        protected GenericBlackboard blackboard => controller.blackboard;
        

        ////////// Utility
        protected StimuliFilter GetEnemyFilter() => stimuliSettings.GetEnemyFilter();
        protected StimuliFilter GetAllyFilter() => stimuliSettings.GetAllyFilter();
        protected StimuliFilter GetNeutralFilter() => stimuliSettings.GetNeutralFilter();
        protected StimuliFilter GetNoiseFilter() => stimuliSettings.GetNoiseFilter();
        protected StimuliFilter GetPainFilter() => stimuliSettings.GetPainFilter();
        protected StimuliFilter GetTouchFilter() => stimuliSettings.GetTouchFilter();
    }
}
