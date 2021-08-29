using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    /*public class NoiseSourceVelocity : MonoBehaviour
    {
        [InfoBox("3D not impemented")]
        public float noiseThreshold;
        public float noiseAccumulatorFallPerSecond = 1.0f;
        [Space] public float hitVelocityScale = 1.0f;
        public float walkVelocityScale = 1.0f;
        float _noiseAccumulator;

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!collision.rigidbody)
                return;

            PerceiveUnit unit = collision.gameObject.GetComponent<PerceiveUnit>();
            if (!unit)
                return;

            _noiseAccumulator += collision.rigidbody.velocity.magnitude * hitVelocityScale;

            if (_noiseAccumulator > noiseThreshold && NoiseManager.Instance.CanSpreadNoise())
            {
                _noiseAccumulator = 0;

                NoiseData noiseData = new NoiseData();
                noiseData.position = collision.transform.position;
                noiseData.velocity = collision.rigidbody.velocity;
                noiseData.fraction = unit.fraction;

                NoiseManager.Instance.SpreadNoise(noiseData);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!collision.attachedRigidbody)
                return;

            PerceiveUnit unit = collision.GetComponent<PerceiveUnit>();
            if (!unit)
                return;

            _noiseAccumulator += collision.attachedRigidbody.velocity.magnitude * walkVelocityScale;

            if (_noiseAccumulator > noiseThreshold && NoiseManager.Instance.CanSpreadNoise())
            {
                _noiseAccumulator = 0;

                NoiseData noiseData = new NoiseData();
                noiseData.position = collision.transform.position;
                noiseData.velocity = collision.attachedRigidbody.velocity;
                noiseData.fraction = unit.fraction;

                NoiseManager.Instance.SpreadNoise(noiseData);
            }

        }

        void FixedUpdate()
        {
            _noiseAccumulator -= noiseAccumulatorFallPerSecond * Time.fixedDeltaTime;
            if (_noiseAccumulator < 0)
                _noiseAccumulator = 0;
        }
    }*/
}
