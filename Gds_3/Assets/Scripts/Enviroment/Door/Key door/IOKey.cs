using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOKey : MonoBehaviour
{
    private bool addOnce = true;

    [SerializeField] private KeyItemController keyItem;

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
                        Destroy(gameObject);
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
                addOnce = false;
            }
        }
    }
}
