using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DamageOnCollision))]
public class BulletController : BulletControllerBase, IDamagable
{    
    public Timer destroyTime;
    public float reverseForce;  
    public float colorChangeSpeed;
    public float destroySpeed = 1.0f;
    public float damageEnableSpeed = 3.0f;
    public float colorEnableSpeed = 7.0f;

    DamageOnCollision _damageOnCollision;

    [BoxGroup(AttributeHelper.HEADER_CALLBACKS)] public UnityEvent onDestroy;


    private void OnDisable()
    {
        onDestroy.Invoke();
    }

    protected void Start()
    {
        _damageOnCollision = GetComponent<DamageOnCollision>();
        _damageOnCollision.damageDataContinous.instigator = instigator;
        _damageOnCollision.damageDataEnter.instigator = instigator;
        _damageOnCollision.damageDataOnce.instigator = instigator;
    }

    protected new void OnSpawn()
    {
        base.OnSpawn();

        _damageOnCollision = GetComponent<DamageOnCollision>();
        _damageOnCollision.damageDataContinous.instigator = instigator;
        _damageOnCollision.damageDataEnter.instigator = instigator;
        _damageOnCollision.damageDataOnce.instigator = instigator;
        destroyTime.Restart();
        i = 0;
    }

    int i = 0;
    private void FixedUpdate()
    {
        if(destroyTime.IsReady())
        {
            gameObject.SetActive(false);
        }

        if(++i > 5 && _rb.velocity.sqrMagnitude < destroySpeed.Sq())
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }

    public void ReceiveDamage(DamageData data)
    {
        if (!data.ActivateInteractiveObjects)
            return;

        if (!instigator && data.instigator)
        {
            var inputHolder = data.instigator.GetComponentInParent<InputHolder>();
            if (inputHolder)
            {
                _rb.AddForce(data.instigator.transform.forward * reverseForce, ForceMode.Acceleration);
            }
        }
        else if(instigator && data.instigator != instigator)
        {
            Vector3 toInstigator = instigator.transform.position - transform.position;
            _rb.AddForce(toInstigator.normalized * reverseForce, ForceMode.Acceleration);

            instigator = data.instigator;
        }
    }
}

