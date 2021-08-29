using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class DisableOnDeath : MonoBehaviour
{
    public List<Behaviour> disableComponentList;
    public List<Behaviour> destroyComponentList;
    public List<GameObject> disableObjectsList;
    public List<Collider> colliderList;
    public List<GameObject> destroyGameObjectList;
    public List<Transform> deparentObjects;
    public bool disableRigidbody;

    [Space]
    public List<Behaviour> enableComponents;
    public List<Collider> enableColliders;
    public List<GameObject> enableObjectsList;


    private void Start()
    {
        var health = GetComponent<HealthController>();
        health.onDeathCallback += (data) =>
        {
            foreach(var it in disableComponentList)
            {
                it.enabled = false;
            }
            foreach (var it in destroyGameObjectList)
            {
                Destroy(it);
            }
            foreach (var it in destroyComponentList)
            {
                Destroy(it);
            }
            foreach (var it in colliderList)
            {
                it.enabled = false;
            }
            foreach (var it in disableObjectsList)
            {
                it.SetActive(false);
            }
            
            foreach (var it in deparentObjects)
            {
                it.parent = null;
            }

            if (disableRigidbody)
            {
                var rb = GetComponent<Rigidbody>();
                if(rb)
                {
                    Destroy(rb);
                }
            }

            foreach (var it in enableComponents)
            {
                it.enabled = true;
            }
            foreach (var it in enableColliders)
            {
                it.enabled = true;
            }
            foreach (var it in enableObjectsList)
            {
                it.SetActive(true);
            }
        };
    }
}
