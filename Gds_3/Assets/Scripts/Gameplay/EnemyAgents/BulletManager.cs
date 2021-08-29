using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BulletManager : MonoSingleton<BulletManager>
{
    [SerializeField] ObjectPool[] bulletPools;

    public BulletControllerBase SpawnBullet(GameObject instigator, Vector3 position, Quaternion orientation, int bulletId = 0)
    {
        BulletControllerBase bullet = null; ;
        bulletPools[bulletId].SpawnObject((obj) =>
        {
            bullet = obj.GetComponent<BulletControllerBase>();
            Debug.Assert(bullet);

            bullet.instigator = instigator;

            bullet.transform.position = position;
            bullet.transform.rotation = orientation;
        });
        return bullet;
    }
}
