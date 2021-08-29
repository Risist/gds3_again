using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Motor : MonoBehaviour
{
    public float force;
    public float forceFallof = 1.0f;
    public float initialForce;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        //rb.AddForce(transform.forward * initialForce, ForceMode.Force);
    }
    private void FixedUpdate()
    {
        rb.AddForce(transform.forward * force, ForceMode.Force);
        force *= forceFallof;
    }


    void OnSpawn()
    {
        rb.AddForce(transform.forward * initialForce, ForceMode.Force);
    }
}
