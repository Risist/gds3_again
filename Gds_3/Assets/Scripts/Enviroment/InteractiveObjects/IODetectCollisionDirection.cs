using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IODetectCollisionDirection : IODetectDirection
{
    public bool onCollision = false;
    public bool onTrigger = true;

    private void OnTriggerStay(Collider other)
    {
        if (!onTrigger)
            return;

        InputHolder inputHolder = other.GetComponent<InputHolder>();
        if (inputHolder)
        {
            if (other.isTrigger == false)
            {
                RunMostSimilar(transform.position - other.transform.position);
            }
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (!onCollision || other.collider.isTrigger)
            return;

        InputHolder inputHolder = other.gameObject.GetComponent<InputHolder>();
        if (inputHolder)
        {
            RunMostSimilar(transform.position - other.transform.position);
        }
    }
}