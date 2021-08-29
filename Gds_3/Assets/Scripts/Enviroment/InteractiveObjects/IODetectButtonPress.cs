using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IODetectButtonPress : MonoBehaviour
{
    public UnityEvent unityEvent;
    public int inputId = 2;

    private void OnTriggerStay(Collider collision)
    {
        InputHolder inputHolder = collision.GetComponent<InputHolder>();
        if (inputHolder && inputHolder.keys[inputId])
        {
            unityEvent.Invoke();
        }
    }
}
