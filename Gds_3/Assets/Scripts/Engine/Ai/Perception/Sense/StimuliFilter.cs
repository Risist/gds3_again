using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    public class StimuliFilter
    {
        public StimuliFilter(StimuliStorage storage, Transform transform,
               Func<MemoryEvent, float> measureMethod)
        {
            Debug.Assert(storage != null);
            Debug.Assert(measureMethod != null);

            this._storage = storage;
            this._measureMethod = measureMethod;

            this.transform = transform;

        }

        // filters by distance from center
        public StimuliFilter(StimuliStorage storage, Transform _transform,
            float distanceUtilityScale = 0.0f, float timeUtilityScale = 1.0f, float maxLifetime = float.MaxValue)
        {
            Debug.Assert(storage != null);

            this.transform = _transform;

            this._storage = storage;
            this._measureMethod = (MemoryEvent memoryEvent) =>
            {
                if (memoryEvent.elapsedDistance > maxLifetime)
                    return float.MinValue;

                float timeUtility = -memoryEvent.elapsedTime;
                float distanceUtility = -(transform.position - memoryEvent.position).ToPlane().magnitude;

                float utility = timeUtility * timeUtilityScale + distanceUtility * distanceUtilityScale;
                return utility;
            };
        }

        // filters by distance from local position
        public StimuliFilter(StimuliStorage storage, Transform _transform,
            Vector2 localPoint, float distanceUtilityScale = 0.0f, float timeUtilityScale = 1.0f, float maxLifetime = float.MaxValue)
        {
            Debug.Assert(storage != null);

            this.transform = _transform;

            this._storage = storage;
            this._measureMethod = (MemoryEvent memoryEvent) =>
            {
                if (memoryEvent.elapsedDistance > maxLifetime)
                    return float.MaxValue;

                float timeUtility = -memoryEvent.elapsedTime;
                float distanceUtility = -(transform.TransformPoint(localPoint) - memoryEvent.position).ToPlane().magnitude;

                float utility = timeUtility * timeUtilityScale + distanceUtility * distanceUtilityScale;
                return utility;
            };
        }

        Transform transform;
        StimuliStorage _storage;
        Func<MemoryEvent, float> _measureMethod;


        float _lastFrame = -1;
        MemoryEvent _lastEvent;

        // event cache mechanism
        // used to ensure only one memory search will be performed
        // Time.time stays constant throughout frame so we can check if the value has changed since
        // if soo we update stored event
        protected void UpdateEvent()
        {
            if (Time.time != _lastFrame)
            {
                _lastFrame = Time.time;
                _lastEvent = _storage.FindBestEvent(_measureMethod);
                if (_lastEvent != null)
                    Debug.DrawLine(_lastEvent.exactPosition, _lastEvent.position, Color.yellow, Time.fixedDeltaTime, false);
            }
        }

        public MemoryEvent GetTarget()
        {
            UpdateEvent();
            return _lastEvent;
        }

        #region Utility functions

        public float elapsedTime => HasTarget() ? GetTarget().elapsedTime : float.MaxValue;

        public bool HasTarget() { return GetTarget() != null; }
        public Vector3 GetTargetPosition() => HasTarget() ? GetTarget().position : Vector3.zero;
        public Vector3 ToTarget() => GetTargetPosition() - transform.position;
        public Vector2 ToTarget2D() => ToTarget().To2D();

        public Vector3 GetAwayFromTargetPosition(float distance)
        {
            return transform.position - ToTarget() * distance;
        }
        public bool IsInRange(float min, float max)
        {
            float distanceSq = ToTarget2D().sqrMagnitude;
            return distanceSq > min * min && distanceSq < max * max;
        }
        public bool IsAtBehindTargetPosition(float tolerance, float distanceToTargetScale = 1.0f)
        {
            Vector2 toDestination = (BehindTargetPosition(distanceToTargetScale) - transform.position).To2D();
            float distanceSq = toDestination.sqrMagnitude;
            return distanceSq < tolerance * tolerance;
        }
        public bool IsBehindTarget(float tolerance = 0)
        {
            Vector2 targetForward = GetTarget().forward.To2D();
            Vector2 toTarget = ToTarget().To2D();

            return Vector2.Dot(targetForward, toTarget) <= tolerance;
        }
        public bool IsCloser(float distance)
        {
            float distanceSq = ToTarget2D().sqrMagnitude;
            return distanceSq < distance * distance;
        }
        public bool IsFurther(float distance)
        {
            float distanceSq = ToTarget2D().sqrMagnitude;
            return distanceSq > distance * distance;
        }

        public Vector2 ToTargetSide2D(float preference = 0.5f)
        {
            var toTarget = ToTarget2D();
            toTarget = new Vector2(-toTarget.y, toTarget.x);
            return UnityEngine.Random.value > preference ? toTarget : -toTarget;
        }
        public Vector3 ToTargetSide(float preference = 0.5f)
        {
            return ToTargetSide2D(preference).To3D();
        }

        public Vector2 StayInRange2D(float min, float max)
        {
            Vector2 toTarget = ToTarget2D();
            float distanceSq = toTarget.sqrMagnitude;

            if (distanceSq < min * min)
            {
                return -toTarget;
            }
            else if (distanceSq > max * max)
            {
                return toTarget;
            }

            return Vector2.zero;
        }
        public Vector2 StayInRange2D(RangedFloat range)
        {
            return StayInRange2D(range.min, range.max);
        }

        // returns movement direction vector that will try to avoid the target
        // distanceToTargetScale - if 1 will try to stay at the same distance
        //  if less will try to move closer
        public Vector2 StayBehindTargetAdaptive2D(float closeDist = 2.0f, float avoidance = 600,
            float distanceToTargetScale = 1.0f)
        {
            Vector2 toTarget = ToTarget2D();
            float distanceFromTarget = toTarget.magnitude;

            Vector3 destination =
                GetTargetPosition() - GetTarget().forward.normalized * distanceFromTarget * distanceToTargetScale;
            Vector2 toDestination = (destination - transform.position).To2D();

            toDestination = toDestination - toTarget.normalized / toTarget.sqrMagnitude * avoidance * distanceToTargetScale;
            toDestination = toDestination.sqrMagnitude > closeDist * closeDist ? toDestination : Vector2.zero;

            return toDestination;
        }

        public Vector3 StayBehindTargetAdaptive(float closeDist = 2.0f, float avoidance = 600,
            float distanceToTargetScale = 1.0f)
        {
            return StayBehindTargetAdaptive2D(closeDist, avoidance, distanceToTargetScale).To3D();
        }

        public Vector2 StayBehindTargetConstant2D(float closeDist = 2.0f, float avoidance = 600, float distanceToTarget = 10.0f)
        {
            Vector2 toTarget = ToTarget2D();
            Vector3 destination = GetTargetPosition() - GetTarget().forward.normalized * distanceToTarget;
            Vector2 toDestination = (destination - transform.position).To2D();

            toDestination = toDestination - toTarget.normalized / toTarget.sqrMagnitude * avoidance;
            toDestination = toDestination.sqrMagnitude > closeDist * closeDist ? toDestination : Vector2.zero;

            return toDestination;
        }
        public Vector3 StayBehindTargetConstant(float closeDist = 2.0f, float avoidance = 600, float distanceToTarget = 10.0f)
        {
            return StayBehindTargetConstant2D(closeDist, avoidance, distanceToTarget).To3D();
        }

        public Vector3 BehindTargetPosition(float distanceToTargetScale = 1.0f)
        {
            Vector2 toTarget = ToTarget2D();
            float distanceFromTarget = toTarget.magnitude;

            return GetTargetPosition() - GetTarget().forward.normalized * distanceFromTarget * distanceToTargetScale;
        }
        public Vector3 BehindTargetPosition2D()
        {
            return BehindTargetPosition().To2D();
        }

        #endregion
    }
}
