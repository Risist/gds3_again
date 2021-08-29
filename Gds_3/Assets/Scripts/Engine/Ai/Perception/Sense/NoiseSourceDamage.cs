using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{

    /*[RequireComponent(typeof(HealthController))]
    public class NoiseSourceDamage : MonoBehaviour
    {
        public float damageThreshold;
        public float damageAccumulatorFallPerSecond = 1.0f;
        float _damageAccumulator;

        void Start()
        {
            var healthController = GetComponent<HealthController>();
            healthController.onDamageCallback += (DamageData data) =>
            {
                _damageAccumulator += data.damage;

                if (_damageAccumulator >= damageThreshold && SenseNoise.CanSpreadNoise())
                {
                    _damageAccumulator = 0;

                    NoiseData noiseData = new NoiseData();
                    noiseData.position = data.position;
                    noiseData.velocity = Vector2.zero;

                    SenseNoise.SpreadNoise(noiseData);
                }
            };


        }

        void FixedUpdate()
        {
            _damageAccumulator -= damageAccumulatorFallPerSecond * Time.fixedDeltaTime;
            if (_damageAccumulator < 0)
                _damageAccumulator = 0;
        }

    }*/
}
