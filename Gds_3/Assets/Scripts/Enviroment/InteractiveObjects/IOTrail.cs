using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class IOTrail : MonoBehaviour
{
    public int poolId;

    public void SpawnTrail()
    {
        TrailManager.Instance.SpawnTrail(transform, poolId);
    }
}
