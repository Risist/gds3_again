using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IODetectDamage : MonoBehaviour, IDamagable 
{
    public UnityEvent unityEvent;
    public void ReceiveDamage(DamageData data)
    {
        if (data.ActivateInteractiveObjects == false)
            return;
        if(data.ActivateInteractiveObjects)
        {
            unityEvent.Invoke();
        }
    }
}
