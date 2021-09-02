using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class IORepetetiveCall : MonoBehaviour
{
    public UnityEvent call;
    public Timer timer;

    private void Update()
    {
        if(timer.IsReadyRestart())
        {
            call.Invoke();
        }
    }
}
