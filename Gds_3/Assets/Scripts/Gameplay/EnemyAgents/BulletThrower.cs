using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BulletThrower : MonoBehaviour
{
    [Serializable]
    public struct BulletData
    {
        public int bulletId;
        public Transform transform;
    }
    [SerializeField] private BulletData[] bulletDatas; 

    public void ThrowBullet(int i = 0)
    {
        var bulletData = bulletDatas[i];
        Debug.Assert(bulletData.transform);
        BulletManager.Instance.SpawnBullet(gameObject, bulletData.transform.position, bulletData.transform.rotation, bulletData.bulletId);
    }

}
