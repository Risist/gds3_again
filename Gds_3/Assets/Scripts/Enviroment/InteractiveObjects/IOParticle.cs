using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

public class IOParticle : MonoBehaviour
{
    public int particleId;

    public void SpawnParticle()
    {
        var particle = ParticleManager.Instance.SpawnParticle(transform, particleId);
        particle.Play();
    }
}
