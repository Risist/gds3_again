using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthController : MonoBehaviour, IDamagable
{
    [Header("Health")]
    public float currentHealth = 100;
    public float maxHealth = 100;

    public bool destroyOnDeath = true;

    public Action<DamageData> onDamageCallback  = (DamageData data) => { };
    public Action<DamageData> onDeathCallback   = (DamageData data) => { };
    public Action<DamageData> onStaggerCallback = (DamageData data) => { };

    [Header("Stagger")]
    public Timer tResetStagger;
    public float staggerLimit;
    public float staggerLevel;

    bool destroyed = false;

    public void Ressurect()
    {
        destroyed = false;
        currentHealth = maxHealth;
    }

    public void ReceiveDamage(DamageData data)
    {
        currentHealth -= data.damage;
        staggerLevel += data.staggerIncrease;


        if (data.staggerIncrease > 0)
        {
            tResetStagger.Restart();
        }

        if(staggerLevel >= staggerLimit)
        {
            staggerLevel -= staggerLimit;
            onStaggerCallback(data);
        }

        if (currentHealth > 0)
            onDamageCallback(data);
        else if (!destroyed)
        {
            onDeathCallback(data);
            destroyed = true;

            if (destroyOnDeath)
                Destroy(gameObject);
        }
    }

    void Update()
    {
        if(tResetStagger.IsReadyRestart())
        {
            staggerLevel = Mathf.Clamp(staggerLevel - 1, 0, int.MaxValue);
        }
    }
}
