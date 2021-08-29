using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DeathAnimation : MonoBehaviour
{
    public Vector3 targetRotation;
    public float animationSpeed = 10;
    Coroutine coroutine;
    private void Start()
    {
        var health = GetComponent<HealthController>();
        Debug.Assert(health);
        health.onDeathCallback += (data) =>
        {
            if(coroutine == null)
                coroutine = StartCoroutine(AnimationCoroutine());
        };
    }

    IEnumerator AnimationCoroutine()
    {
        Vector3 rotation = transform.eulerAngles;
        while (! targetRotation.To2D().Approximately(rotation.To2D()))
        {
            rotation.x = Mathf.Lerp(rotation.x, targetRotation.x, animationSpeed * Time.deltaTime);
            rotation.z = Mathf.Lerp(rotation.z, targetRotation.z, animationSpeed * Time.deltaTime);

            transform.eulerAngles = rotation;

            yield return null;
        }

        //coroutine = null;
    }

}

