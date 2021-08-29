using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class IODestroy : MonoBehaviour
{
    public GameObject objectToDestroy;
    public float timeToDestroy;

    public void DestroyObject()
    {
        Destroy(objectToDestroy, timeToDestroy);
    }

}
