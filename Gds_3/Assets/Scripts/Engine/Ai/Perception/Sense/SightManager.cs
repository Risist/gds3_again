using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    public class SightManager : MonoSingleton<SightManager>
    {
        static public new SightManager Instance => GetInstance(
            autoCreateReference: false,
            findReference: true,
            logErrorIfNotResolved: true);

        public readonly List<SenseSight> _sightList = new List<SenseSight>();

        public LayerMask memorableMask;
        public LayerMask obstacleMask;
        [Space]
        public float searchTime = 0.1f;
        public bool enable2DSight = true;
        public bool enable3DSight = false;
        
        // in order for automatic search to occur for 3d colliders this coroutine has to be called 
        /*public IEnumerator PerformSearch_Coroutine()
        {
            var wait = new WaitForSeconds(searchTime);
            while (true)
            {
                yield return wait;

                foreach (var it in _sightList)
                    it.PerformSearch();
            }
        }*/

        public void AddSight(SenseSight sight)
        {
            _sightList.Add(sight);
        }
        public void RemoveSight(SenseSight sight)
        {
            _sightList.Remove(sight);
        }

        // in order for automatic search to occur for 2d colliders this coroutine has to be called
        /*public IEnumerator PerformSearch2D_Coroutine()
        {
            var wait = new WaitForSeconds(searchTime);
            while (true)
            {
                yield return wait;

                foreach (var it in _sightList)
                    it.PerformSearch2D();
            }
        }*/
        private new void OnEnable()
        {
            base.OnEnable();

            //if (enable3DSight)
            //    StartCoroutine(PerformSearch_Coroutine());

            /*if (enable3DSight)
                StartCoroutine(PerformSearch_Coroutine());*/
        }
    }
}
