using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IOArmatActivator : MonoBehaviour
{
    public UnityEvent unityEvent;
    public bool onEnter = true;
    public bool onStay = true;
    public int waitTimer = 2;
    public bool isPressed = true;

    private void OnTriggerEnter(Collider other)
    {
        if (onEnter)
        {
            InputHolder inputHolder = other.GetComponent<InputHolder>();
            if (inputHolder && inputHolder.keys[2] && isPressed)
            {
                unityEvent.Invoke();
                isPressed = false;
                StartCoroutine(PauseInterection());
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (onStay)
        {
            InputHolder inputHolder = other.GetComponent<InputHolder>();
            if (inputHolder && inputHolder.keys[2] && isPressed)
            {
                unityEvent.Invoke();
                isPressed = false;
                StartCoroutine(PauseInterection());
            }
        }
    }

    private IEnumerator PauseInterection()
    {
        yield return new WaitForSeconds(waitTimer);
        isPressed = true;
    }
}
