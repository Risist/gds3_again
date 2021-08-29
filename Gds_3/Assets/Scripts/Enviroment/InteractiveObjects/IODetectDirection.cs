using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class IODetectDirection : MonoBehaviour
{
    [Serializable]
    public class DirectionRecord
    {
#if UNITY_EDITOR
        [SerializeField] string name;

        public override string ToString()
        {
            return name;
        }
        public Color directionColor = Color.red;
#endif
        public Vector3 direction;

        public UnityEvent action = null;
    }
    public List<DirectionRecord> directions;


    public void OnDrawGizmos()
    {
#if UNITY_EDITOR
        foreach (var it in directions)
        {
            Gizmos.color = it.directionColor;
            Gizmos.DrawRay(transform.position, transform.TransformDirection(it.direction));
        }
#endif
    }

    protected void RunMostSimilar(Vector3 hitNormal)
    {
        DirectionRecord best = null;
        float leastDot = -999;
        foreach (var it in directions)
        {
            float dot = Vector2.Dot(transform.TransformDirection(it.direction.normalized).To2D().normalized, hitNormal.To2D().normalized);
            if (dot > leastDot)
            {
                leastDot = dot;
                best = it;
            }
        }

        best?.action.Invoke();
    }
}
