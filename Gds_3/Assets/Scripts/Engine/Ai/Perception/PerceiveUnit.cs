using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Marker for objects captured by perception system
 */
namespace Ai
{
    public class PerceiveUnit : MonoBehaviour
    {
        [Tooltip("modifies how far the agents will perceive this unit")]
        public float distanceModificator = 1.0f;

        [Range(0, 1), Tooltip("modifies how far (%) agents will be able to see through the object")]
        public float transparencyLevel = 1.0f;

        public Fraction fraction;

        [Tooltip("list of obstacle collision raycast targets\n"
               + "if empty adds this object to the list")]
        public Transform[] obstacleTestTargets;

        private void Awake()
        {
            if (obstacleTestTargets.Length == 0)
                obstacleTestTargets = new Transform[]{ transform };
        }
    }
}
