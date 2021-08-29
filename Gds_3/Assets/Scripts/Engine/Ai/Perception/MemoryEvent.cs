using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{

    /*
     * Stores singular information about perceived event
     */
    [System.Serializable]
    public class MemoryEvent
    {
        public MinimalTimer lifetimeTimer = new MinimalTimer();

        /// position of the event source at record time
        public Vector3 exactPosition;

        /// facing direction of the event; Vector.zero if not applicable
        public Vector3 forward;

        /// direction the movement proceeds when perceived
        public Vector3 velocity;

        /// unity responsible for this event or null if unknown or none
        public PerceiveUnit perceiveUnit;


        /// distance which target could possibly travel from the start of the event assuming same speed
        public float elapsedDistance => velocity.magnitude * lifetimeTimer.ElapsedTime();

        public float elapsedTime => lifetimeTimer.ElapsedTime();

        /// fully computed position predicted by this event
        /// comes up with uncertainity area, and linear interpolation of current position by direction and speed of the event
        public Vector3 position => exactPosition + velocity * elapsedTime.Sq() / (elapsedTime.Sq() + 1);

        public Vector3 GetPositionNotScaled(float maxRespectedTime = float.MaxValue, float timeScale = 1.0f)
        {
            float time = lifetimeTimer.ElapsedTime();
            time = Mathf.Clamp(time, 0, maxRespectedTime);

            return exactPosition + timeScale * velocity * time / (time + 1);
        }
    }
}
