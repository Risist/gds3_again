using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class IODeparent : MonoBehaviour
{
    public Transform[] objects;

    public void Deparent()
    {
        foreach(var it in objects)
        {
            it.parent = null;
        }
    }

}
