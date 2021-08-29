using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IODestructibleWall : MonoBehaviour
{
    [SerializeField] GameObject destroyed;

    private void Start()
    {
        var health = GetComponent<HealthController>();
        health.onDeathCallback += (DamageData data) =>
        {
            Instantiate(destroyed, transform.position, transform.rotation);
            Destroy(gameObject);
        };
    }
}
