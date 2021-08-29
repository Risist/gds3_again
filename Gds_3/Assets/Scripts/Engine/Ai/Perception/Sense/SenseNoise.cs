using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using NaughtyAttributes;

namespace Ai
{
    public class NoiseData
    {
        public Vector2 position;
        public Vector2 velocity;
        public Fraction fraction;
    }

    public class SenseNoise : SenseBase
    {
        [BoxGroup(AttributeHelper.HEADER_DETECTING)]
        [Range(0, 1)] public float reactionChance = 1.0f;

        [BoxGroup(AttributeHelper.HEADER_DETECTING)]
        public float hearingDistance = 1.0f;

        void ReactToNoise(NoiseData data)
        {
            // do not record ally or neutral noises
            if (data.fraction && myUnit.fraction.GetAttitude(data.fraction) != Fraction.EAttitude.EEnemy)
                return;

            // check if the noise is in right range to be heared
            Vector2 toNoise = (Vector2) transform.position - data.position;
            if (toNoise.sqrMagnitude > hearingDistance * hearingDistance || Random.value > reactionChance)
                return;

            MemoryEvent ev = new MemoryEvent();
            ev.velocity = data.velocity * stimuliSettings.noiseStorageSettings.velocityPredictionScale;
            ev.forward = data.velocity;
            ev.exactPosition = data.position;
            ev.lifetimeTimer.Restart();

            stimuliSettings.noiseStorage.PerceiveEvent(ev);

            Debug.DrawRay(ev.exactPosition, Vector3.up, Color.blue, 0.5f);
        }

        void OnEnable()  => NoiseManager.Instance.AddNoiseListener(ReactToNoise);
        void OnDisable() => NoiseManager.UnsafeInstance?.RemoveNoiseListener(ReactToNoise);
    }
}