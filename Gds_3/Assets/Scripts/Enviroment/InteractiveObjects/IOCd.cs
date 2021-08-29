using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


public class IOCd : MonoBehaviour
{
    public Timer cd;
    public UnityEvent ifReady;


    public void Restart()
    {
        cd.Restart();
    }

    public void Check()
    {
        if(cd.IsReady())
        {
            ifReady.Invoke();
        }
    }

}
