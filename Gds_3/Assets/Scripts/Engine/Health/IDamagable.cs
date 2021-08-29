using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageData
{
    public float damage;
    public float staggerIncrease;
    [SerializeField] bool activateInteractiveObjects = true;
    public bool ActivateInteractiveObjects => activateInteractiveObjects && damage > 0;
    [System.NonSerialized] public Vector3 position;
    [System.NonSerialized] public Vector3 direction;
    [System.NonSerialized] public GameObject instigator;
}

// interface for all objects that can be damaged
public interface IDamagable
{
    void ReceiveDamage(DamageData data);
}
