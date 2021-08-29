using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IODetectCollision : MonoBehaviour
{
    public UnityEvent unityEvent;

    public bool onEnter = true;
    public bool onStay = true;
    public bool onTriggerEnter = true;
    public bool onTriggerStay = true;


    private void OnCollisionEnter(Collision collision)
    {
        if (onEnter)
        {
            InputHolder inputHolder = collision.gameObject.GetComponent<InputHolder>();
            if (inputHolder)
            {
                unityEvent.Invoke();
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (onStay)
        {
            InputHolder inputHolder = collision.gameObject.GetComponent<InputHolder>();
            if (inputHolder)
            {
                unityEvent.Invoke();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (onTriggerEnter)
        {
            InputHolder inputHolder = other.gameObject.GetComponent<InputHolder>();
            if (inputHolder)
            {
                unityEvent.Invoke();
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (onTriggerStay)
        {
            InputHolder inputHolder = other.gameObject.GetComponent<InputHolder>();
            if (inputHolder)
            {
                unityEvent.Invoke();
            }
        }
    }
}
