using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class TrailManager : MonoSingleton<TrailManager>
{
    public ObjectPool[] trailPools;


    public TrailRenderer SpawnTrail(Transform followObject, int poolId = 0)
    {
        TrailRenderer trail = null;
        trailPools[poolId].SpawnObject((obj) =>
        {
            obj.transform.position = followObject.position;
            obj.transform.rotation = followObject.rotation;
            trail = obj.GetComponent<TrailRenderer>();
            TrailController trailController = obj.GetComponent<TrailController>();
            Debug.Assert(trail);
            Debug.Assert(trailController);
            trailController.followObject = followObject;
            var trailTime = trail.time;

            // ugh.. trailRenderer.Clear does not work, its some kind of hack to persuade it slightly
            trail.time = 0;
            trail.Clear();
            obj.transform.position = followObject.position;
            trail.time = 0;
            trail.Clear();

            trail.time = trailTime;
        });
        trail.Clear();

        return trail;
    }
}
