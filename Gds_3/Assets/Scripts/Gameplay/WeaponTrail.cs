using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

public class WeaponTrail : MonoBehaviour
{
    [Required] public TrailRenderer trail;
    float trailTime;

    private void Awake()
    {
        trail.transform.parent = null;
        trailTime = trail.time;
    }

    private void OnEnable()
    {
        trail.time = 0;
        trail.Clear();
        trail.transform.position = transform.position;
        trail.Clear();
        trail.transform.position = transform.position;
        trail.time = trailTime;
    }

    private void Update()
    {
        trail.transform.position = transform.position;
    }
}
