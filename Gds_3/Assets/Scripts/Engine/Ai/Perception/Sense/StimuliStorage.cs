using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    // Stores stimuli collected by given senses
    // behaviours can then read them directly or through focus
    [Serializable]
    public class StimuliStorage
    {
        public StimuliStorage(float maxEventLifetime, int nEvents = 5)
        {
            _memoryEvents = new MemoryEvent[nEvents];
            this.maxEventLifetime = maxEventLifetime;
            _lastAddedEvent = -1;
        }

        public float maxEventLifetime { get; private set; }
        [SerializeField, ReadOnly] MemoryEvent[] _memoryEvents;
        int _lastAddedEvent = -1;

        // callback called each time a new memory event is stored
        public Action<MemoryEvent> onEventPerceived = (ev) => { };

        // pushes event onto perceived events list
        // restarts lifetimeTimer
        public void PerceiveEvent(MemoryEvent newEvent)
        {
            Debug.Assert(newEvent != null, "Trying to register null as MemoryEvent");

            // check if in memory there is any event caused by the same unit as newEvent
            // that way we will store only one event per unit
            if (newEvent.perceiveUnit)
            {
                for (int i = 0; i < _memoryEvents.Length; ++i)
                    if (_memoryEvents[i] != null && _memoryEvents[i].perceiveUnit == newEvent.perceiveUnit)
                    {
                        _memoryEvents[i] = null;
                        break;
                    }
            }

            _lastAddedEvent = (_lastAddedEvent + 1) % _memoryEvents.Length;
            _memoryEvents[_lastAddedEvent] = newEvent;
            newEvent.lifetimeTimer.Restart();
            onEventPerceived(newEvent);
        }

        // returns best event from registered ones by measureEventMethod
        // the one with greatest evaluation value will be returned or null if none is valid
        // Events are considered as valid if they are not null and they are not older than maxEventLifeTime
        public MemoryEvent FindBestEvent(Func<MemoryEvent, float> measureEventMethod)
        {
            MemoryEvent bestEvent = null;
            float bestUtility = float.MinValue;

            foreach (var it in _memoryEvents)
            {
                if (it == null || it.elapsedTime > maxEventLifetime)
                    continue;


                float utility = measureEventMethod(it);
                if (utility > bestUtility)
                {
                    bestUtility = utility;
                    bestEvent = it;
                }
            }

            return bestEvent;
        }
    }
}
