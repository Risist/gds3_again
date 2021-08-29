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

    private void OnDisable()
    {
        onDisable.Invoke();
    }
    private void OnEnable()
    {
        onEnable.Invoke();
    }
}
