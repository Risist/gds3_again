using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

public class TrailController : MonoBehaviour
{
    [Required] public Transform followObject;
    [Required] public TrailRenderer trailRenderer;
    public Timer tToDisable;

    bool lastTimeEnabled;
    float trailTime = -1;

    private void Awake()
    {
        if (trailTime < 0)
        {
            trailTime = trailRenderer.time;
        }
    }

    protected void OnSpawn()
    {
        if (trailTime < 0)
        {
            trailTime = trailRenderer.time;
        }

        // ugh.. trailRenderer.Clear does not work, its some kind of hack to persuade it slightly
        trailRenderer.time = 0;
        trailRenderer.Clear();
        trailRenderer.transform.position = followObject.position;
        trailRenderer.Clear();
        trailRenderer.transform.position = followObject.position;
        trailRenderer.time = trailTime;

        Debug.Assert(trailRenderer.positionCount == 0);
        lastTimeEnabled = true;
    }

    private void Update()
    {
        if(lastTimeEnabled)
        {
            if (!followObject || followObject.gameObject.activeInHierarchy == false)
            {
                lastTimeEnabled = false;
                tToDisable.Restart();
            }
        }else if(tToDisable.IsReady())
        {
            // return to pool
            gameObject.SetActive(false);

            trailRenderer.time = 0;
            trailRenderer.Clear();
            trailRenderer.time = 0;
        }

        transform.position = followObject.position;
        transform.rotation = followObject.rotation;
    }
}
