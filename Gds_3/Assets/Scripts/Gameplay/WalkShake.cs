using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class WalkShake : MonoBehaviour
{
    public float shakeStrenght;

    CameraController cameraController;

    private void Start()
    {
        cameraController =
        Camera.main.GetComponent<CameraController>();
    }

    public void ApplyWalkShake()
    {
        cameraController.ApplyShake(Vector3.up * shakeStrenght);
    }
}
