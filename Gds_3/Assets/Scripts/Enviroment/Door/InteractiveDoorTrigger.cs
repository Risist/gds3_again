using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveDoorTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private string openAnim = null;
    private bool isOpen = false;
    public UnityEvent pressedEvent;

    private void OnTriggerStay(Collider other)
    {
        InputHolder inputHolder = other.GetComponent<InputHolder>();

        if(inputHolder && inputHolder.keys[2])
        {
            isOpen = true;
            animator.Play(openAnim, 0, 0.0f);
            pressedEvent.Invoke();
        }
    }
}
