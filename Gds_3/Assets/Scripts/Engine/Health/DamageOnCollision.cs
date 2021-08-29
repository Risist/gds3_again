using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageOnCollision : MonoBehaviour
{
    /// To prevent multiplay hit counts per attack this list will keep track of targets already damaged
    /// So they wont be damaged anymore unless next attack sequence occurs
    protected List<IDamagable> damaged = new List<IDamagable>();

    public DamageData damageDataOnce;
    public DamageData damageDataContinous;
    public DamageData damageDataEnter;
    public GameObject instigator;

    public UnityEvent onDamageDealed;

    public bool damageOnCollision = true;
    public bool damageOnTrigger = true;

    public Ai.Fraction damageFraction;

    protected void Awake()
    {
        if (!instigator)
            instigator = gameObject;

        damageDataOnce.instigator = instigator;
        damageDataContinous.instigator = instigator;
        damageDataEnter.instigator = instigator;

        lastPosition = transform.position.To2D();

        if (!damageFraction)
        {
            damageFraction = GetComponentInParent<Ai.PerceiveUnit>()?.fraction;
        }
    }

    protected Vector2 lastPosition;
    protected void FixedUpdate()
    {
        lastPosition = transform.position.To2D();
    }
    


    protected void OnTriggerStay(Collider other)
    {
        if (!enabled)
            return;

        if (!damageOnTrigger)
            return;

        if (instigator == other.gameObject)
            return; // do not kill yourself

        if (damageFraction != null && other.GetComponentInChildren<Ai.PerceiveUnit>() is Ai.PerceiveUnit perceiveUnit)
        {
            if (damageFraction.GetAttitude(perceiveUnit.fraction) != Ai.Fraction.EAttitude.EEnemy)
            {
                return;
            }
        }

        var damagable = other.GetComponent<IDamagable>();
        damageDataEnter.position = transform.position;
        damageDataContinous.position = transform.position;
        damageDataOnce.position = transform.position;

        if (damagable != null)
        {
            damagable.ReceiveDamage(damageDataContinous);
            if (AttemptToDamage(damagable))
                damagable.ReceiveDamage(damageDataOnce);

            onDamageDealed.Invoke();
        }
    }
    protected void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        if (!damageOnTrigger)
            return;

        if (instigator == other.gameObject)
            return; // do not kill yourself

        if (damageFraction != null && other.GetComponentInChildren<Ai.PerceiveUnit>() is Ai.PerceiveUnit perceiveUnit)
        {
            if (damageFraction.GetAttitude(perceiveUnit.fraction) != Ai.Fraction.EAttitude.EEnemy)
            {
                return;
            }
        }

        var damagable = other.GetComponent<IDamagable>();
        damageDataEnter.position = transform.position;
        damageDataContinous.position = transform.position;
        damageDataOnce.position = transform.position;
        
        if (damagable != null)
        {
            damagable.ReceiveDamage(damageDataEnter);
            if (AttemptToDamage(damagable))
                damagable.ReceiveDamage(damageDataOnce);

            onDamageDealed.Invoke();
        }
    }

    protected void OnCollisionStay(Collision collision)
    {
        if (!enabled)
            return;

        if (!damageOnCollision)
            return;

        if (instigator == collision.gameObject)
            return; // do not kill yourself

        if (damageFraction != null && collision.gameObject.GetComponentInChildren<Ai.PerceiveUnit>() is Ai.PerceiveUnit perceiveUnit)
        {
            if (damageFraction.GetAttitude(perceiveUnit.fraction) != Ai.Fraction.EAttitude.EEnemy)
            {
                return;
            }
        }

        var damagable = collision.gameObject.GetComponent<IDamagable>();
        damageDataEnter.position = transform.position;
        damageDataContinous.position = transform.position;
        damageDataOnce.position = transform.position;
        if (damagable != null)
        {
            damagable.ReceiveDamage(damageDataContinous);
            if (AttemptToDamage(damagable))
                damagable.ReceiveDamage(damageDataOnce);

            onDamageDealed.Invoke();
        }
    }
    protected void OnCollisionEnter(Collision collision)
    {
        if (!enabled)
            return;

        if (!damageOnCollision)
            return;

        if (instigator == collision.gameObject)
            return; // do not kill yourself

        if (damageFraction != null && collision.gameObject.GetComponentInChildren<Ai.PerceiveUnit>() is Ai.PerceiveUnit perceiveUnit)
        {
            if (damageFraction.GetAttitude(perceiveUnit.fraction) != Ai.Fraction.EAttitude.EEnemy)
            {
                return;
            }
        }

        var damagable = collision.gameObject.GetComponent<IDamagable>();
        damageDataEnter.position = transform.position;
        damageDataContinous.position = transform.position;
        damageDataOnce.position = transform.position;
        if (damagable != null)
        {
            damagable.ReceiveDamage(damageDataEnter);
            if (AttemptToDamage(damagable))
                damagable.ReceiveDamage(damageDataOnce);

            onDamageDealed.Invoke();
        }
    }

    /// tells if it is possible to deal damage to target
    /// and records that damage had meed done
    public bool AttemptToDamage(IDamagable target)
    {
        foreach (var it in damaged)
            if (it == target)
                return false;
        damaged.Add(target);
        return true;
    }

    protected void OnEnable()
    {
        damaged.Clear();
    }
}
