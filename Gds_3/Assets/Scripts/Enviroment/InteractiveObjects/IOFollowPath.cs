using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine.Events;

public class IOFollowPath : MonoBehaviour
{
    public Transform ObjectToMove;

    public float speed = 5f;
    public float acceleration = 0f;
    public float rotationSpeed = 5f;
    public Vector3 rotationOffset;

    public UnityEvent unityEvent;
    public UnityEvent startPath;

    private float distance;
    private bool canMove = false;
    private bool isMoving = false;

    private BGCcMath math;
    private BGCurve curve;


    private void Start()
    {
        curve = GetComponentInParent<BGCurve>();
        math = GetComponentInParent<BGCcMath>();
    }
    void Update()
    {
        if (canMove)
        {
            FollowPosition();
            if (!isMoving)
            {
                startPath.Invoke();
                isMoving = true;
            }
        }
    }
    private void FixedUpdate()
    {
        if (canMove)
        {
            speed += acceleration;
        }
    }
    public void StartFollowPosition()
    {
        canMove = true;
    }

    void FollowPosition()
    {
        distance += speed * Time.deltaTime;
        Vector3 tangAtSplineCenter;
        ObjectToMove.position = math.CalcPositionAndTangentByDistance(distance, out tangAtSplineCenter);
        var targetRotation = Quaternion.LookRotation(tangAtSplineCenter) * Quaternion.Euler(rotationOffset);
        var newRotation = Quaternion.Slerp(ObjectToMove.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        ObjectToMove.rotation = newRotation;


        var lastPoint = curve[curve.PointsCount - 1];
        if (ObjectToMove.position.Approximately(lastPoint.PositionWorld))
        {
            unityEvent.Invoke();
        }
    }
}
