using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    public abstract class SenseBase : MonoBehaviour
    {
        protected PerceiveUnit myUnit;
        protected BehaviourController behaviourController;
        protected MonoStimuliSettings stimuliSettings;


        protected void Awake()
        {
            myUnit = GetComponentInParent<PerceiveUnit>();
            behaviourController = GetComponentInParent<BehaviourController>();
            stimuliSettings = GetComponentInParent<MonoStimuliSettings>();
        }
    }
}