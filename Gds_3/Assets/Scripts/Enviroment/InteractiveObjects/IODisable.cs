using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class IODisable : MonoBehaviour
{
    public UnityEvent onDisable;
    public UnityEvent onEnable;
    public bool AvoidFirstDisable = false;

    bool alreadyDisabled = false;

    private void OnDisable()
    {
        if (!AvoidFirstDisable || alreadyDisabled) 
        {
            onDisable.Invoke();
        }
        alreadyDisabled = true;
    }
    private void OnEnable()
    {
        onEnable.Invoke();
    }
}
