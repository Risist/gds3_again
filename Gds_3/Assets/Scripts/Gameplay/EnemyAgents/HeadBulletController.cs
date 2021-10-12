using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

[DefaultExecutionOrder(999999)]
public class HeadBulletController : BulletControllerBase, IDamagable
{
    [Required] public Transform graphicsTransform;
    public Timer destroyTime;
    public Timer hitActivationTime;
    public float reverseForce;

    public DamageData damage;
    public float maxSpeedForYPosition;
    public float rotationSpeed;

    float initialY;

    [Header("Physics")]
    [Range(0, 1)] public float velocityDrag;

    Vector3 velocity;


    bool shouldDealDmg => instigator;

    protected new void Awake()
    {
        base.Awake();

        damage.instigator = instigator;
    }

    protected new void OnSpawn()
    {
        base.OnSpawn();

        initialY = transform.position.y;

        damage.instigator = instigator;
        
        destroyTime.Restart();
        hitActivationTime.Restart();

        graphicsTransform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        if (destroyTime.IsReady())
        {
            gameObject.SetActive(false);
        }

        Vector3 position = transform.position;
        const float idleGroundTime = 1.5f;
        float time = Mathf.Clamp01(destroyTime.ElapsedTime() / (destroyTime.cd - idleGroundTime) );
        float desiredY = initialY * 0.75F + Mathf.Sin(Mathf.PI * 0.5f + time * Mathf.PI ) * initialY * 0.9f;
        
        position.y = Mathf.Lerp(position.y, desiredY, maxSpeedForYPosition);
        position.y = Mathf.Lerp(position.y, desiredY, maxSpeedForYPosition);

        //position += velocity * Time.deltaTime;
        //velocity *= velocityDrag;

        transform.position = position;

        if(instigator)
        {
            Quaternion desiredRotation = Quaternion.FromToRotation(Vector3.up, _rb.velocity);
            graphicsTransform.rotation = Quaternion.Slerp(graphicsTransform.rotation, desiredRotation, rotationSpeed);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (instigator == collision.gameObject)
            return;

        if(shouldDealDmg)
        {
            var damagable = collision.gameObject.GetComponent<IDamagable>();
            damage.position = transform.position;

            if (instigator)
            {
                var instigatorUnit = instigator.GetComponent<Ai.PerceiveUnit>();
                var collisionUnit = collision.gameObject.GetComponent<Ai.PerceiveUnit>();
                if (instigatorUnit && collisionUnit)
                {
                    if (instigatorUnit.fraction.GetAttitude(collisionUnit.fraction) != Ai.Fraction.EAttitude.EEnemy)
                    {
                        return;
                    }
                }
            }

            if (damagable != null)
            {
                damagable.ReceiveDamage(damage);
            }

            
            gameObject.SetActive(false);
        }
    }

    public void ActivateBullet()
    {
        hitActivationTime.Restart();
        destroyTime.Restart();
    }

    public void ReceiveDamage(DamageData data)
    {
        if (!hitActivationTime.IsReady())
            return;

        if (!data.ActivateInteractiveObjects)
            return;

        if (!data.instigator)
            return;

        //if (!instigator || data.instigator != instigator)
        {
            var inputHolder = data.instigator.GetComponentInParent<InputHolder>();
            if (inputHolder)
            {
                ActivateBullet();
                instigator = data.instigator;
                _rb.AddForce(data.instigator.transform.forward.ToPlane() * reverseForce, ForceMode.Acceleration);
                //velocity += data.instigator.transform.forward.ToPlane() * reverseForce;
            }
        }
            
        
    }
}
