using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using AK.Wwise;
using WWiseEvent = AK.Wwise.Event;

public class WWiseTest : MonoBehaviour
{
    public WWiseEvent ev;

    public void Start()
    {
        ev.Post(gameObject);
    }

}
