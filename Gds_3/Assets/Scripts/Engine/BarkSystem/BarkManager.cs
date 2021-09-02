using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BarkSystem
{
    public class BarkManager : MonoSingleton<BarkManager>
    {
        public ObjectPool[] barkPools;

        public bool CallBark(Transform target, BarkSet barkSet, int poolId = 0)
        {
            float bestUtility = float.MinValue;
            BarkRecord bestRecord = null;

            foreach (var it in barkSet.records)
            {
                float utility = UnityEngine.Random.value * it.utility;
                if (utility > bestUtility)
                {
                    bestUtility = utility;
                    bestRecord = it;
                }
            }

            if (bestRecord == null || bestRecord.text.Equals(""))
            {
                // do not show background of the bark if there is no text
                // "" string will function as a possibility in barkSet to select no text display
                return false;
            }

            var barkInstance = target.GetComponent<BarkInstance>();
            if(barkInstance && barkInstance.currentBarkController)
            {
                return false;
            }


            BarkController bark = null;
            barkPools[poolId].SpawnObject((obj) =>
            {
                bark = obj.GetComponent<BarkController>();
                Debug.Assert(bark);

                bark.target = target;

                if (barkInstance)
                {
                    barkInstance.currentBarkController = bark;
                    bark.barkInstance = barkInstance;
                }
            });
            
            bark.CallBark(bestRecord);
            return true;
            
        }
    }
}
