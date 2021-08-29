using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IODisassembly : MonoBehaviour
{
    public float explosionForce = 0.1f;
    public float explosionRadius = 0.1f;
    public Transform explosionCenter;
    public float destroyColliderTime = 2;
    public float turnOffCollisionTime = 1;
    public Collider[] parts = null;
    public Transform[] deparentObjects;

    public float mass = 1f;
    public float drag = 0f;
    public float angularDrag = 0.5f;


    private bool isDissasembled = false; 

    private void Start()
    {
        foreach (var it in parts)
        {
            it.enabled = false;
        }
        if (!explosionCenter)
        {
            explosionCenter = transform;
        }
    }
    private IEnumerator DestroyCdn()
    {
        yield return new WaitForSeconds(turnOffCollisionTime);
        foreach (var it in parts)
        {
            it.enabled = false;
        }
    }
    public void Dissasembly()
    {
        if(isDissasembled)
        {
            return; 
        }
        isDissasembled = true;
        CoroutineManager.Instance.StartCoroutine(DestroyCdn());
        foreach (var it in parts)
        {
            it.transform.parent = null;
            Debug.Assert(it, "part is null");
            var rb = it.gameObject.AddComponent<Rigidbody>();
            Debug.Assert(rb, "part doesn't have Rigidbody");
            it.enabled = true;
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.AddExplosionForce(explosionForce, explosionCenter.position, explosionRadius);
            Destroy(it.gameObject, destroyColliderTime);
        }
        foreach (var part in deparentObjects)
        {
            part.transform.parent = null;
        }    
        Destroy(gameObject);
    }
}
