using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    public class SenseTouch : SenseBase
    {
        [BoxGroup(AttributeHelper.HEADER_GENERAL)]
        public Timer tTouch = new Timer(0);

        [BoxGroup(AttributeHelper.HEADER_DETECTING)]
        public float minimalRelativeVelocity;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // omit too weak touches
            if (collision.relativeVelocity.sqrMagnitude < minimalRelativeVelocity * minimalRelativeVelocity)
                return;

            // only targets with perceive unit will report
            // mostly to allow to tune what is perceived
            PerceiveUnit otherUnit = collision.gameObject.GetComponent<PerceiveUnit>();
            if (!otherUnit)
                return;


            // only report every given seconds
            if (!tTouch.IsReadyRestart())
                return;

            var otherRigidbody = collision.rigidbody;
            var otherTransform = collision.transform;

            MemoryEvent ev = new MemoryEvent();
            ev.exactPosition = otherTransform.position;
            ev.forward = otherTransform.forward;
            ev.velocity = otherRigidbody ? otherRigidbody.velocity : Vector2.zero;
            ev.lifetimeTimer.Restart();

            stimuliSettings.touchStorage.PerceiveEvent(ev);

            Debug.DrawRay(ev.exactPosition, Vector3.up, Color.blue);
        }
    }
}