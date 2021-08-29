using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BulletControllerBase : MonoBehaviour
{
    [NonSerialized] public GameObject instigator;

    protected Rigidbody _rb;

    protected void Awake()
    {
        _rb = GetComponent<Rigidbody>();

    }

    protected void OnSpawn()
    {
        if (_rb)
        {
            _rb.velocity = Vector3.zero;
        }
    }
}
