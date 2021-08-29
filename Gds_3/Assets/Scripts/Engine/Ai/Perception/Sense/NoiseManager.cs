using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using NaughtyAttributes;

namespace Ai
{
    public class NoiseManager : MonoSingleton<NoiseManager>
    {
        static public new NoiseManager Instance => GetInstance(
            autoCreateReference: false,
            findReference: true,
            logErrorIfNotResolved: true);

        public Timer tSpreadNoise = new Timer(0.25f);
        Action<NoiseData> _onSpreadNoise = (data) => { };

        public bool CanSpreadNoise()
        {
            return tSpreadNoise.IsReady();
        }

        public void SpreadNoise(NoiseData data)
        {
            if (tSpreadNoise.IsReadyRestart())
            {
                _onSpreadNoise(data);
            }
        }

        public void AddNoiseListener(Action<NoiseData> a)
        {
            _onSpreadNoise += a;
        }
        public void RemoveNoiseListener(Action<NoiseData> a)
        {
            _onSpreadNoise -= a;
        }
    }
}
