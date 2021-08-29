using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;
using NaughtyAttributes;

public class CurveRenderer : MonoBehaviour
{
    [SerializeField, Required] private LineRenderer lineRenderer;
    [SerializeField] private int nSections;
    [SerializeField] private Vector3 positionOffset;


    private void Start()
    {
        InitLine();
    }

    [Button]
    void InitLine()
    {
        BGCcMath math = GetComponentInParent<BGCcMath>();

        float fullDistance = math.GetDistance();

        lineRenderer.positionCount = nSections;
        for(int i = 0; i < nSections; ++i)
        {
            float distance = fullDistance * i / (nSections - 1);
            Vector3 position = math.CalcPositionByDistance(distance);
            position.y = 0.01f;
            lineRenderer.SetPosition(i, position + positionOffset);
        }
    }
}
