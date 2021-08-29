using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;


[DefaultExecutionOrder(99999999)]
public class ObjectDetection : MonoBehaviour
{
    [Serializable]
    public class QueryType
    {
#if UNITY_EDITOR
        [SerializeField] string name; // helper comment
#endif
        public LayerMask mask;
        public RangedFloat angleRange;
        [ReadOnly] public float closestDistance;
        [ReadOnly] public float velocity;
        [ReadOnly] public Collider closestCollider;

        public void ResetData()
        {
            closestDistance = float.MaxValue;
            velocity = 0;
        }
    }
    public QueryType[] queries;

    // in order to ensure execution order we need to manually store objects inside a trigger instead of relaying on OnTriggerStay
    // before addition of all the events we need to reset state of all the queries
    List<Collider> collidingObjects = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        int layer = 1 << other.gameObject.layer;
        bool toAny = false;

        foreach (var it in queries)
        {
            if (((layer & it.mask.value) != layer))
                continue;

            toAny = true;
            break;
        }

        if (toAny)
        {
            collidingObjects.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        collidingObjects.Remove(other);
    }

    private void FixedUpdate()
    {
        foreach (var it in queries)
        {
            it.ResetData();
        }

        foreach (var colider in collidingObjects)
        {
            if ( !colider || !colider.enabled || !colider.gameObject.activeInHierarchy)
                continue;
            // compute an angle
            Vector2 toOther = (colider.transform.position - transform.position).To2D();

            float angle = Vector2.SignedAngle(toOther, Vector2.up);

            // compute distance
            float dist = toOther.magnitude;
            // get Layer;
            int layer = 1 << colider.gameObject.layer;

            foreach (var it in queries)
            {
                if (((layer & it.mask.value) != layer))
                    continue;

                if (it.angleRange.InRange(angle))
                    continue;


                if(dist < it.closestDistance)
                {
                    it.closestDistance = dist;
                    it.closestCollider = colider;
                }
                if(colider.attachedRigidbody)
                {
                    it.velocity = Mathf.Max(it.velocity, colider.attachedRigidbody.velocity.magnitude);
                }
            }
        }
        
    }
}
