using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    public class MonoPatrolCenter : MonoBehaviour
    {
        public float radius = 15;


        public Vector3 Clamp(Vector3 position)
        {
            Vector3 center = transform.position;
            Vector3 toCenter = position - center;

            float magnitude = toCenter.magnitude;
            float clampedMagnitude = Mathf.Clamp(magnitude, 0, radius);

            return center + toCenter.normalized * clampedMagnitude;
        }


#if UNITY_EDITOR
        [BoxGroup(AttributeHelper.HEADER_DEBUG)]
        public Color gizmoColor = new Color(0, 1, 0, 0.15f);

        [BoxGroup(AttributeHelper.HEADER_DEBUG)]
        public bool drawGizmo;


        private void OnDrawGizmos()
        {
            if (!drawGizmo)
            {
                return;
            }

            UnityEditor.Handles.color = gizmoColor;
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector2.up, radius);

            UnityEditor.Handles.color = Color.black;
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector2.up, 0.5f);
        }

        private void OnDrawGizmosSelected()
        {
            if (drawGizmo)
            {
                return;
            }

            UnityEditor.Handles.color = gizmoColor;
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector2.up, radius);

            UnityEditor.Handles.color = Color.black;
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector2.up, 0.5f);
        }
#endif
    }
}
