using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// non generic name for its content, but well it's end of the project, almost no use of trying to be smart with names and reusability
public class RotatingTrailAnimation : MonoBehaviour
{
    public float rotationSpeed = 100;
    float lastRotation;

    private void Start()
    {
        lastRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        lastRotation += rotationSpeed * Time.deltaTime;

        Vector3 euler = transform.eulerAngles;
        euler.y = lastRotation;
        transform.eulerAngles = euler;
    }
}
