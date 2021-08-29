using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour
{
    [Tooltip("Target to follow")]
    public Transform target;
    
    [Tooltip("offset from follow point")]
    public Vector3 offset;
    
    [Tooltip("lerp factor of how fast camera follows target"), Range(0.0f, 100.0f)]
    public float followFactor = 0.3f;
    
    [Tooltip("When target distance is clamped to the value")]
    public float maxDifference = float.PositiveInfinity;

    public float directionOffsetMaxLength = float.MaxValue;

    [Header("Shakes")]
    [Range(0, 1)] public float shakeDrag = 0.95f;
    [Range(0, 1)] public float shakeLerp = 0.05f;
    Vector3 _shakeOffset;
    Vector3 _desiredShake;

    public void ApplyShake(Vector3 force)
    {
        _desiredShake += force;
    }
    public void ApplyShakeRandom(float force)
    {
        _desiredShake += Random.insideUnitCircle.To3D() * force;
    }
    public void ApplyShakeRandom2D(float force)
    {
        _desiredShake += (Vector3)Random.insideUnitCircle * force;
    }



    List<Func<Vector3>> directionOffsetList = new List<Func<Vector3>>();
    public void AddDirectionOffset(Func<Vector3> func)
    {
        Debug.Assert(func != null);
        directionOffsetList.Add(func);
    }
    public void RemoveDirectionOffset(Func<Vector3> func)
    {
        Debug.Assert(func != null);
        directionOffsetList.Remove(func);
    }
    public Vector3 GetOffset()
    {
        Vector3 offset = Vector3.zero;
        foreach(var it in directionOffsetList)
        {
            offset += it();
        }
        return Vector3.ClampMagnitude(offset, directionOffsetMaxLength);
    }

    //public Func<Vector3> directionOffset = () => Vector3.zero;

    Transform _cachedTransform;

    void Start()
    {
        _cachedTransform = transform;
        if(!target)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        Debug.Assert(target, "Camera target not set up");
    }

    void FixedUpdate()
    {
        _shakeOffset = Vector3.Lerp(_shakeOffset, _desiredShake, shakeLerp);
        _desiredShake *= shakeDrag;
    }
    Vector3 _lastTargetPosition;

    void LateUpdate()
    {
        // in case target dies
        if (target)
        {
            _lastTargetPosition = target.position;
        }


        // vector to target, counted without offset

        var directionOffsetValue = GetOffset();
        Vector3 toTarget = _cachedTransform.position - offset - _lastTargetPosition - directionOffsetValue - _shakeOffset;
        toTarget.z = 0;
        float toTargetDistSq = toTarget.sqrMagnitude;


        if (toTargetDistSq > maxDifference * maxDifference)
        {
            Vector3 pos = _lastTargetPosition + toTarget.normalized * maxDifference + offset + directionOffsetValue + _shakeOffset;
            _cachedTransform.position = pos;
        }
        _cachedTransform.position = Vector3.Lerp(_cachedTransform.position, _lastTargetPosition + offset + directionOffsetValue + _shakeOffset, followFactor * Time.deltaTime);
    }
}
