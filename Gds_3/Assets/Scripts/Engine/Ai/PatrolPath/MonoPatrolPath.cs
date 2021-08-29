using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    // class for easy editing of patrol paths
    public class MonoPatrolPath : MonoBehaviour
    {
        [Tooltip("cooldown before any other agent can use the path")]
        public Timer tCooldown;

        bool _allreadyTraversed;
        public bool CanBeTraversed => !_allreadyTraversed && tCooldown.IsReady();

        public void StartTraversal()
        {
            _allreadyTraversed = true;
        }
        public void EndTraversal()
        {
            tCooldown.Restart();
            _allreadyTraversed = false;
        }

#if UNITY_EDITOR
        [BoxGroup(AttributeHelper.HEADER_DEBUG)]
        public Color debugLinesColor = Color.green;
        
        [BoxGroup(AttributeHelper.HEADER_DEBUG)]
        public bool drawGizmo;
        
        private void OnDrawGizmos()
        {
            if(!drawGizmo)
            {
                return;
            }

            var cachedTransform = transform;
            for (int i = 0; i < cachedTransform.childCount - 1; ++i)
            {
                Vector3 start = cachedTransform.GetChild(i).position;
                Vector3 end = cachedTransform.GetChild(i + 1).position;
                Debug.DrawLine(start, end, debugLinesColor);
            }

            if (drawGizmo)
            {
                var c = Camera.main;
                for (int i = 0; i < cachedTransform.childCount; ++i)
                {
                    Vector3 pointPos = cachedTransform.GetChild(i).position;
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.red;
                    style.fontSize = 30;
                    UnityEditor.Handles.Label(pointPos, i.ToString(), style );
                }
            }
        }
#endif

        public PatrolPath GetPatrolPath()
        {
            PatrolPath patrolPath = new PatrolPath();
            var cachedTransform = transform;
            for (int i = 0; i < cachedTransform.childCount; ++i)
            {
                patrolPath.pointList.Add(cachedTransform.GetChild(i).position);
            }

            return patrolPath;
        }

    }
}
