using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HeadBullet : MonoBehaviour, IDamagable
{
    public float movementSpeed;
    public Timer tHit; 
    public Timer tWaitForHit;
    public Color activeMaterialColor;

    public LayerMask groundLayer;
    public Ai.Fraction fraction;

    Vector3 movementVector;
    bool alreadyHit = true;

    Rigidbody rb;

    new Renderer renderer;
    Color originalMaterialColor;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        originalMaterialColor = renderer.material.color;

        var health = GetComponentInParent<HealthController>();
        health.onDeathCallback += (DamageData data) =>
        {
            if (!data.instigator)
            {
                Destroy(gameObject);
            }

            Vector3 damagePosition = data.position;
            movementVector = Vector3.zero;// 
            transform.parent = null;
            enabled = true;

            tHit.Restart();
            tWaitForHit.Restart();
            alreadyHit = false;

            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1000;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        };


        enabled = false;
    }

    private void Update()
    {
        Vector3 position = transform.position + movementVector.ToPlane().normalized * movementSpeed * Time.deltaTime;

        if (position.y < 0.5f)
            position.y = 0.5f;

        transform.position = position;



        if(tWaitForHit.IsReady())
        {
            renderer.material.color = Color.Lerp(activeMaterialColor, originalMaterialColor, tHit.ElapsedTime() / tHit.cd);
        }else
        {
            renderer.material.color = Color.Lerp(originalMaterialColor, activeMaterialColor, tHit.ElapsedTime() / tWaitForHit.cd);
        }


    }
    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * 0.35f, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        

        var perceiveUnit     = other.GetComponent<Ai.PerceiveUnit>();
        var healthController = other.GetComponent<IDamagable>();
        if (perceiveUnit && healthController != null &&
            fraction.GetAttitude(perceiveUnit.fraction) == Ai.Fraction.EAttitude.EEnemy)
        {
            if (alreadyHit)
            {
                DamageData data = new DamageData();
                data.damage = 1000; // insta kill
                data.instigator = null;

                healthController.ReceiveDamage(data);

                Destroy(gameObject);
            }
        }
        else if (!perceiveUnit && ((1 << other.gameObject.layer)  & groundLayer.value) == 0)
        {
            Debug.Log(other.gameObject);
            //movementVector = Vector3.zero;
            Destroy(gameObject);
        }
    }

    public void ReceiveDamage(DamageData data)
    {
        if (alreadyHit == false && !tHit.IsReady() && tWaitForHit.IsReady())
        {
            if (data.instigator)
            {
                var inputHolder = data.instigator.GetComponentInParent<InputHolder>();
                if(inputHolder)
                {
                    alreadyHit = true;
                    movementVector = data.instigator.transform.forward;
                    //movementVector = -data.instigator.transform.position + inputHolder.directionInput.To3D() - inputHolder.transform.position;//data.instigator.transform.forward; 
                }
            }
        }
    }
}

