using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorKeyTrigger : MonoBehaviour
{
    public UnityEvent action;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player")
            return;

        InputHolder inputHolder = other.GetComponent<InputHolder>();

        if (inputHolder && inputHolder.keys[2])
        {
            action.Invoke();
        }
    }
}
