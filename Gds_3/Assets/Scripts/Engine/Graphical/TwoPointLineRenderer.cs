using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class TwoPointLineRenderer : MonoBehaviour
{
    public Transform transformFrom; 
    public Transform transformTo;

    LineRenderer _lineRenderer;


    public void Break()
    {
        transformFrom = null;
        transformTo = null;
    }

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void LateUpdate()
    {
        if (transformFrom && transformTo)
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, transformFrom.position - transform.position);
            _lineRenderer.SetPosition(1, transformTo.position - transform.position);
        }
        else
        {
            _lineRenderer.positionCount = 0;
        }
    }

}
