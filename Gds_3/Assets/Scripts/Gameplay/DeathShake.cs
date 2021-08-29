using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class DeathShake : MonoBehaviour
{
    public float shakeStrenght;

    CameraController cameraController;

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        var healtController = GetComponent<HealthController>();
        healtController.onDeathCallback += (data) =>
        {
            cameraController.ApplyShakeRandom(shakeStrenght);
        };
    }
}