using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(HealthController))]
public class DeathAnimationRunner : MonoBehaviour
{
    public string deathAnimationName;
    public float transitionDuration;
    public float normalizedTimeOffset;
    [Required] public Animator animator;

    bool alreadyPlayed;

    private void Start()
    {

        var health = GetComponent<HealthController>();
        Debug.Assert(health);
        Debug.Assert(animator);

        alreadyPlayed = false;

        health.onDeathCallback += (data) =>
        {
            if (alreadyPlayed == false)
            {
                alreadyPlayed = true;
                animator.CrossFade(deathAnimationName, transitionDuration, 0, normalizedTimeOffset);
            }
        };
    }

}
