using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IODetectCollision : MonoBehaviour
{
    public UnityEvent unityEvent;

    [Space]
    public bool onEnter = true;
    public bool onStay = true;
    public bool onTriggerEnter = true;
    public bool onTriggerStay = true;

    [Space]
    public bool playerOnly;


    private void OnCollisionEnter(Collision collision)
    {
        if (onEnter)
        {
            bool canInvoke = playerOnly && collision.gameObject.CompareTag("Player") 
                || collision.gameObject.GetComponent<InputHolder>();
            
            if (canInvoke)
            {
                unityEvent.Invoke();
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (onStay)
        {
            bool canInvoke = playerOnly && collision.gameObject.CompareTag("Player")
                   || collision.gameObject.GetComponent<InputHolder>();
            if (canInvoke)
            {
                unityEvent.Invoke();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (onTriggerEnter)
        {
            bool canInvoke = playerOnly && other.CompareTag("Player")
                   || other.GetComponent<InputHolder>();
            if (canInvoke)
            {
                unityEvent.Invoke();
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (onTriggerStay)
        {
            bool canInvoke = playerOnly && other.CompareTag("Player")
                   || other.GetComponent<InputHolder>();
            if (canInvoke)
            {
                unityEvent.Invoke();
            }
        }
    }
}
