using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class VisibilityStreaming : MonoBehaviour
{
    public float activateDistance;
    public float deactivateDistance;
    public Transform[] streamingObjects;

    Transform player;
    private float cachedDeactivateDistance;

    private IEnumerator Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        Debug.Assert(player);

        /// :< sad but works
        cachedDeactivateDistance = deactivateDistance;
        deactivateDistance = 10000;

        yield return null;
        /// :(
        /// dont judge me 
        deactivateDistance = cachedDeactivateDistance;
    }


    private void LateUpdate()
    {
        Debug.Log(player.position);
        foreach(var it in streamingObjects)
        {
            if (it == null)
                continue;

            float distanceSq = (it.position - player.position).To2D().sqrMagnitude;
            if(it.gameObject.activeInHierarchy)
            {
                if(distanceSq > deactivateDistance.Sq())
                {
                    it.gameObject.SetActive(false);
                }
            }else
            {
                if (distanceSq < activateDistance.Sq())
                {
                    it.gameObject.SetActive(true);
                }
            }
        }
    }

}
