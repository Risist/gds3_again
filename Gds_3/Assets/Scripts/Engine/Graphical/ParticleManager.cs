using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ParticleManager : MonoSingleton<ParticleManager>
{
    public ObjectPool[] particlePools;


    public ParticleSystem SpawnParticle(Transform spawnPoint, int poolId = 0)
    {
        ParticleSystem particle = null;
        particlePools[poolId].SpawnObject((obj) =>
        {
            obj.transform.position = spawnPoint.position;
            obj.transform.rotation = spawnPoint.rotation;
            particle = obj.GetComponent<ParticleSystem>();
            Debug.Assert(particle);
        });

        return particle;
    }
}
