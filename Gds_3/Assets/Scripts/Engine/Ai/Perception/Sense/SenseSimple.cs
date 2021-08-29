using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


namespace Ai
{
    // TODO WIP
    // Takes exact target dragged from editor and gains omnipresent perception to that target
    /*public abstract class SenseSimple : SenseBase
    {
        const string simpleStorageName = nameof(simpleStorageName);

        [BoxGroup(AttributeHelper.HEADER_GENERAL)]
        public string blackboardKey = simpleStorageName;

        [BoxGroup(AttributeHelper.HEADER_DETECTING)]
        public PerceiveUnit targetUnit;

        public StimuliStorage storage { get; protected set; }
        new void Awake()
        {
            base.Awake();

            //// initialize focus
            storage = RegisterSenseInBlackboard(blackboardKey);
        }

        private void Update()
        {
            MemoryEvent ev = new MemoryEvent();
            ev.exactPosition = targetUnit.transform.position;
            ev.forward = targetUnit.transform.forward;
            ev.lifetimeTimer.Restart();
            ev.perceiveUnit = targetUnit;

            storage.PerceiveEvent(ev);

            Debug.DrawRay(ev.exactPosition, Vector3.up, Color.blue);
        }
    }*/
}