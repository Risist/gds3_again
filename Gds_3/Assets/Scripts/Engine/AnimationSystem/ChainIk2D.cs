using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
        "Uses gradient descent ik for chain case;" +
        "Children are nodes of ik; " +
        "Equally Spaced nodes"*/
//[ExecuteInEditMode]
public class ChainIk2D : MonoBehaviour
{
    [Tooltip("Distance nodes are spaced; first node is connected to this transform")]
    public float nodeLength;
    [Tooltip("distance of first node from the transform")]
    public float firstNodeLength;

    [Tooltip("Allows to set up center of node"), Range(-0.5f, 0.5f)]
    public float nodePartition;

    [Space]
    [Tooltip(""), Range(0.0f, 180.0f)]
    public float maxAngleDifference;
    [Tooltip(""), Range(0.0f, 180.0f)]
    public float firstNodeMaxAngleDifference;
    [Space]

    [Tooltip("Lerp factor representing rotation speed of nodes following desired position"), Range(0.0f, 1.0f)]
    public float rotationSpeed;

    Transform[] nodes;
    new Transform transform;

    void Start()
    {
        transform = base.transform;
        nodes = new Transform[transform.childCount];
        for (int i = nodes.Length-1; i >= 0; --i)
        {
            nodes[i] = transform.GetChild(i);
        }
        transform.DetachChildren();
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateNode(0, transform, firstNodeLength, firstNodeMaxAngleDifference);
        for(int i = 1; i < nodes.Length; ++i)
        {
            UpdateNode(i, nodes[i - 1], nodeLength, maxAngleDifference);
        }
    }

    void UpdateNode(int i, Transform target, float nodeLength, float maxAngleDifference)
    {
        Transform node = nodes[i];

        {
            Vector2 offset = node.position - target.position;

            float angle = Vector2.SignedAngle(Vector2.up, offset);
            angle = MathfE.ClampAngle(angle, target.eulerAngles.z - maxAngleDifference, target.eulerAngles.z + maxAngleDifference);
            Quaternion rotation = Quaternion.Euler(0,0,angle);

            Vector3 desiredPosition = target.position + rotation*Vector3.up * nodeLength;
            node.position = desiredPosition;
        }

        {
            Vector2 offset = node.position + target.forward*nodeLength * nodePartition - target.position;

            float angle = Vector2.SignedAngle(Vector2.up, offset);
            angle = MathfE.ClampAngle(angle, target.eulerAngles.z - maxAngleDifference, target.eulerAngles.z + maxAngleDifference);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            // set rotation
            node.rotation = Quaternion.Lerp(node.rotation, rotation, rotationSpeed);
        }
    }

    
}
namespace UnityEngine
{
    public static class MathfE
    {
        public static float ClampAngle( float angle, float min, float max)
        {
            float start = (min + max) * 0.5f - 180;
            float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
            min += floor;
            max += floor;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
