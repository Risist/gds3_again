using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    public class SensePain : SenseBase
    {
        [BoxGroup(AttributeHelper.HEADER_GENERAL)]
        public Timer tPain = new Timer(0);

        new void Awake()
        {
            base.Awake();

            //// initialize focus
            var healthController = GetComponentInParent<HealthController>();

            healthController.onDamageCallback += (data) =>
            {
                if (!tPain.IsReadyRestart())
                    return;

                MemoryEvent ev = new MemoryEvent();
                ev.exactPosition = data.position;
                ev.forward = data.direction;
                // we have no information about hit velocity; just assume it is stationary
                ev.velocity = Vector2.zero;
                ev.lifetimeTimer.Restart();

                stimuliSettings.painStorage.PerceiveEvent(ev);

                Debug.DrawRay(ev.position, Vector3.up, Color.blue, 0.25f);
            };
        }

    }
}