using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IOKey : MonoBehaviour
{
    public UnityEvent keyPickupEvent;

    private bool addOnce = true;

    [SerializeField] private KeyItemController keyItem;
    InteractableObjectGUI objectGUI;

    private void Start()
    {
        objectGUI = GetComponentInChildren<InteractableObjectGUI>();
    }

    private void OnTriggerStay(Collider other)
    {
        InputHolder inputHolder = other.GetComponent<InputHolder>();
        if (inputHolder)
        {
            if (inputHolder && inputHolder.keys[2])
            {
                if (keyItem.keys != 2)
                {
                    if (addOnce)
                    {
                        keyItem.keys = keyItem.keys + 1;
                        objectGUI.TurnOffOutline();
                        keyPickupEvent.Invoke();
                        gameObject.SetActive(false);
                        
                    }
                }
                else
                {
                    objectGUI.TurnOffOutline();
                    gameObject.SetActive(false);
                   
                }
                addOnce = false;
            }
        }
    }
}
