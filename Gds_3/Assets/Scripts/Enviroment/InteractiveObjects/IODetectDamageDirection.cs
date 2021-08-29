using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Enviroment.InteractiveObjects
{
    public class IODetectDamageDirection : IODetectDirection, IDamagable
    {
        public void ReceiveDamage(DamageData data)
        {
            if (!data.ActivateInteractiveObjects)
                return;

            RunMostSimilar(transform.position - data.position);
        }
    }
}
