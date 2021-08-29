using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FadeOnCharacterCollision : MonoBehaviour
{
    public float alphaChangeSpeed;
    public float insideAlpha;

    public MeshRenderer[] renderers;
    List<Collider> collidersInside = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        var holder = other.GetComponent<InputHolder>();
        if (holder)
        {
            collidersInside.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collidersInside.Remove(other);
    }

    private void LateUpdate()
    {
        var target = collidersInside.Count == 0 ? 1.0f : insideAlpha;

        foreach (var it in renderers)
        {
            var color = it.material.color;

            color.a = Mathf.Lerp(color.a, target, alphaChangeSpeed * Time.deltaTime);
            it.material.color = color;
        }
    }

    public void DestroyAfterTurnOn()
    {
        StartCoroutine(DestroyAfterTurnOnCoroutine());
    }

    IEnumerator DestroyAfterTurnOnCoroutine()
    {
        bool allAtDesiredA = false;
        while(!allAtDesiredA)
        {
            allAtDesiredA = true;
            foreach (var it in renderers)
            {
                var color = it.material.color;

                color.a = Mathf.MoveTowards(color.a, 1.0f, alphaChangeSpeed * Time.deltaTime);
                it.material.color = color;

                allAtDesiredA &= Mathf.Approximately(color.a, 1.0f);
            }

            yield return null;
        }

        Destroy(gameObject);
    }

}
