using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class IOTimer : MonoBehaviour 
{
    public UnityEvent onCompleted;
    public Timer timer;

    bool alreadyCalled = true;

    public void Restart()
    {
        if (alreadyCalled)
        {
            timer.Restart();
            alreadyCalled = false;
        }
    }

    private void Update()
    {
        if(!alreadyCalled && timer.IsReady())
        {
            alreadyCalled = true;
            onCompleted.Invoke();
        }
    }
}
