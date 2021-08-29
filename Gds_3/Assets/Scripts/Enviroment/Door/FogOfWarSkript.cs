using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarSkript : MonoBehaviour
{
    [SerializeField] private bool fadeOut = false;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private GameObject fadeInObject = null;
    Coroutine coroutineFadeIn;
    Coroutine coroutineFadeOut;
    new Renderer renderer;


    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        GameEvents.Instance.fadeInObject += FadeInDoorOpen;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (coroutineFadeIn != null)
            {
                StopCoroutine(coroutineFadeIn);
            }
            coroutineFadeIn = StartCoroutine(FadeInObject());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (coroutineFadeOut != null)
            {
                StopCoroutine(coroutineFadeOut);
            }
            StartCoroutine(FadeOutObject());
        }
    }

    public void FadeInDoorOpen(GameObject room)
    {
        if (room == fadeInObject)
        {
            StartCoroutine(FadeInObject());
        }
    }

    private IEnumerator FadeInObject()
    {
        Color objectColor;
        while (renderer.material.color.a > 0)
        {
            objectColor = renderer.material.color;
            float fadeAmout = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor.a = fadeAmout;
            renderer.material.color = objectColor;
            coroutineFadeIn = null;
            yield return null;
        }
        renderer.enabled = false;
    }
    private IEnumerator FadeOutObject()
    {
        if (fadeOut)
        {
            Color alphaColor = renderer.material.color;
            alphaColor.a = 0;
            while (renderer.material.color.a < 1)
            {
                Color objectColor = renderer.material.color;
                float fadeAmout = objectColor.a + (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmout);
                renderer.material.color = objectColor;
                coroutineFadeOut = null;
                yield return null;
            }
            renderer.enabled = true;
        }
    }
}
