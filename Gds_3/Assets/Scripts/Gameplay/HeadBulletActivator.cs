using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class HeadBulletActivator : MonoBehaviour
{
    public int bulletId = 1;
    public float initialForce;
    [Range(-1, 1)] public float bulletSpawnDot = 1f;

    private void Start()
    {
        var health = GetComponentInParent<HealthController>();

        health.onDeathCallback += (data) =>
        {
            if (data.instigator)
            {
                Vector3 instigatorDirection = data.instigator.transform.forward;
                Vector3 forward = transform.forward;

                if (Vector3.Dot(instigatorDirection, forward) < bulletSpawnDot)
                {
                    var bullet = BulletManager.Instance.SpawnBullet(null, transform.position, transform.rotation, bulletId);
                }
            }

            /*var bullet = BulletManager.Instance.SpawnBullet(null, transform.position, transform.rotation, bulletId);
            var rb = bullet.GetComponent<Rigidbody>();
            if(data.instigator && rb)
            {
                Vector3 instigatorDirection = data.instigator.transform.forward;
                Vector3 forward = transform.forward;

                if (Vector3.Dot(instigatorDirection, forward) < 0)
                {
                    if (bullet is HeadBulletController b)
                    {
                        b.ActivateBullet();
                    }
                    rb.AddForce(data.instigator.transform.forward.ToPlane() * initialForce, ForceMode.Acceleration);
                }
            }*/
            //Destroy(gameObject);
        };
    }

}
