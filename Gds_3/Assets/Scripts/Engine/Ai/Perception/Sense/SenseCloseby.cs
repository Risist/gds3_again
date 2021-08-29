using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace Ai
{
    [Serializable]
    public class TimedDetection
    {

        public class DetectionRecord
        {
            public PerceiveUnit unit;
            public MinimalTimer tSeen = new MinimalTimer();
        }
        readonly List<DetectionRecord> _detectionRecords = new List<DetectionRecord>();

        public void AddRecord(PerceiveUnit unit)
        {
            if (_detectionRecords.Find((r) => r.unit == unit) == null)
            {
                var record = new DetectionRecord();
                record.tSeen.Restart();
                record.unit = unit;
                _detectionRecords.Add(record);
            }
        }
        public void RemoveRecord(PerceiveUnit unit)
        {
            if (unit)
            {
                _detectionRecords.RemoveAll((u) => u.unit == unit);
            }
        }

        public void IterateOver(Action<DetectionRecord> processEvent)
        {
            foreach (var it in _detectionRecords)
            {
                if (it.unit)
                {
                    processEvent(it);
                }
            }
        }
    }

    public class SenseCloseby : SenseBase
    {
        public const string HEADER_VISION = "VisionSettings"; 
        public const string HEADER_SHAPE = "Shape";

        [BoxGroup(HEADER_VISION)] public bool trackEnemy        = true;
        [BoxGroup(HEADER_VISION)] public bool trackAlly         = false;
        [BoxGroup(HEADER_VISION)] public bool trackNeutrals     = false;

        [BoxGroup(AttributeHelper.HEADER_DEBUG), ReadOnly] public StimuliStorage enemyStorage;// { get; protected set; }
        [BoxGroup(AttributeHelper.HEADER_DEBUG), ReadOnly] public StimuliStorage allyStorage; // { get; protected set; }
        [BoxGroup(AttributeHelper.HEADER_DEBUG), ReadOnly] public StimuliStorage neutralStorage;// { get; protected set; }

        [BoxGroup(HEADER_VISION)] public float timeToBeDetected;
        TimedDetection _timedDetection = new TimedDetection();

        // insert given event to the storage specified by an attitiude
        void InsertEvent(MemoryEvent ev, Fraction.EAttitude attitude)
        {
            switch (attitude)
            {
                case Fraction.EAttitude.EEnemy:
                    if (trackEnemy)
                        stimuliSettings.enemyStorage.PerceiveEvent(ev);
                    break;
                case Fraction.EAttitude.EFriendly:
                    if (trackAlly)
                        stimuliSettings.allyStorage.PerceiveEvent(ev);
                    break;
                case Fraction.EAttitude.ENeutral:
                    if (trackNeutrals)
                        stimuliSettings.neutralStorage.PerceiveEvent(ev);
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            bool b = ((1 << other.gameObject.layer) & SightManager.Instance.memorableMask) != 0;
            if (b)
            {
                PerceiveUnit perceiveUnit = other.GetComponent<PerceiveUnit>();
                if (perceiveUnit == myUnit)
                    // oh, come on do not look at yourself... don't be soo narcissistic
                    return;

                if (!perceiveUnit)
                    // no perceive unit, this target is invisible to us
                    return;

                Fraction itFraction = perceiveUnit.fraction;
                if (!itFraction)
                    // the same as above,
                    return;

                _timedDetection.AddRecord(perceiveUnit);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            var unit = other.GetComponent<PerceiveUnit>();
            if (unit)
            {
                _timedDetection.RemoveRecord(unit);
            }
        }

        private void FixedUpdate()
        {
            _timedDetection.IterateOver((record) =>
            {
                var unit = record.unit;
                if (!record.tSeen.IsReady(timeToBeDetected))
                    return;

                //// determine attitude
                Fraction myFraction = myUnit.fraction;
                Fraction itFraction = unit.fraction;
                Fraction.EAttitude attitude = myFraction.GetAttitude(itFraction);

                //// prepare an event
                MemoryEvent ev = new MemoryEvent();
                ev.exactPosition = unit.transform.position;
                ev.forward = unit.transform.forward;
                ev.velocity = Vector2.zero;
                ev.lifetimeTimer.Restart();
                ev.perceiveUnit = unit;

                //// push it into the right storage
                InsertEvent(ev, attitude);
            });
        }
    }
}
