using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOShake : MonoBehaviour
{
    CameraController cameraController;

    public RangedFloat shakeForce;
    public float shakeInterval = 0.1f;
    public float shakeDelay = 0;
    public int nunmerOfShakes = 1;
    private Coroutine coroutine;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }
    public void RunShake()
    {
        if (coroutine == null)
        {
            coroutine = CoroutineManager.Instance.StartCoroutine(RunShakeCoroutine());
        }
    }
    IEnumerator RunShakeCoroutine()
    {
        yield return new WaitForSeconds(shakeDelay);
        for (int i = 0; i < nunmerOfShakes; ++i)
        {
            cameraController.ApplyShakeRandom(shakeForce.GetRandom());
            yield return new WaitForSeconds(shakeInterval);
        }
        coroutine = null;
    }
}
