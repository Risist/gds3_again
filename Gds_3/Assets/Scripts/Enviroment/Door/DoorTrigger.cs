using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorTrigger : MonoBehaviour
{
    public UnityEvent events;

    private void OnTriggerEnter(Collider other)
    {
        InputHolder inputHolder = other.GetComponent<InputHolder>();
        if (inputHolder)
        {
            events.Invoke();
        }
    }
}