using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCollisionMovement : MonoBehaviour
{
    public DamageData damageData;
    public GameObject instigator;

    public Ai.Fraction damageFraction;

    public float damageAngle = 180.0f;

    List<GameObject> objectsInside = new List<GameObject>();

    Vector3 lastPosition;

    bool IsOnMovementDirection(Vector3 otherPosition)
    {
        Vector2 toCurrentPosition = (transform.position.To2D() - lastPosition.To2D()).normalized;
        Vector2 toOther = (transform.position - otherPosition).To2D().normalized;

        float dot = Vector2.Dot(toCurrentPosition, toOther);
        float angle = Mathf.Acos(dot);

        return angle < damageAngle * 0.5f;
    }

    private void Start()
    {
        if (!instigator)
        {
            instigator = gameObject;
        }

        if (!damageFraction)
        {
            damageFraction = GetComponentInParent<Ai.PerceiveUnit>()?.fraction;
        }

        lastPosition = transform.position;
        damageData.instigator = instigator;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var damagable = collision.gameObject.GetComponent<IDamagable>();

        if (damagable != null && collision.collider.isTrigger == false)
        {
            objectsInside.Add(collision.gameObject);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        objectsInside.Remove(collision.gameObject);
    }

    private void FixedUpdate()
    {
        if (!enabled)
        {
            lastPosition = transform.position;
            return;
        }

        if (lastPosition.To2D().Approximately(transform.position.To2D()))
        {
            lastPosition = transform.position;
            return;
        }

        foreach (var it in objectsInside)
        {
            if (it == null)
                continue;

            if (it == instigator)
                continue;

            /*if (!IsOnMovementDirection(it.transform.position))
                continue;*/ 

            if (damageFraction != null && it.GetComponentInChildren<Ai.PerceiveUnit>() is Ai.PerceiveUnit perceiveUnit)
            {
                if (damageFraction.GetAttitude(perceiveUnit.fraction) != Ai.Fraction.EAttitude.EEnemy)
                {
                    continue;
                }
            }

            damageData.position = transform.position;

            var damagable = it.GetComponent<IDamagable>();
            damagable?.ReceiveDamage(damageData);
        }

        lastPosition = transform.position;
    }
}
