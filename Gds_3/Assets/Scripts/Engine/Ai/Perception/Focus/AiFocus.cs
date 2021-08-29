using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    /*
     */
    /*public abstract class FocusBase
    {
        protected FocusBase(Transform myTransform)
        {
            this.transform = myTransform;
        }

        public Transform transform;

        #region Virtual

        public abstract MemoryEvent GetTarget();

        public virtual bool HasTarget()
        {
            return GetTarget() != null;
        }

        public virtual Vector2 GetTargetPosition()
        {
            return HasTarget() ? GetTarget().position : Vector2.zero;
        }

        public virtual Vector2 ToTarget()
        {
            return GetTargetPosition() - (Vector2)transform.position;
        }

        #endregion Virtual

        #region Utility Functions

        public Vector2 GetAwayFromTargetPosition(float distance)
        {
            return (Vector2)transform.position - ToTarget()* distance;
        }

        public bool IsInRange(float min, float max)
        {
            float distanceSq = ToTarget().sqrMagnitude;
            return distanceSq > min * min && distanceSq < max * max;
        }

        public bool IsBehindTarget(float tolerance)
        {
            Vector2 toDestination = BehindTargetPosition() - (Vector2) transform.position;
            float distanceSq = toDestination.sqrMagnitude;
            return distanceSq < tolerance * tolerance;
        }

        public bool IsCloser(float distance)
        {
            float distanceSq = ToTarget().sqrMagnitude;
            return distanceSq < distance * distance;
        }

        public bool IsFurther(float distance)
        {
            float distanceSq = ToTarget().sqrMagnitude;
            return distanceSq > distance * distance;
        }

        public Vector2 ToTargetSide(float preference = 0.5f)
        {
            var toTarget = ToTarget();
            toTarget = new Vector2(-toTarget.y, toTarget.x);
            return UnityEngine.Random.value > preference ? toTarget : -toTarget;
        }

        public Vector2 StayInRange(float min, float max)
        {
            Vector2 toTarget = ToTarget();
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

        public Vector2 StayInRange(RangedFloat range)
        {
            return StayInRange(range.min, range.max);
        }

        public Vector2 StayBehindTargetAdaptive(float closeDist = 2.0f, float avoidance = 600,
            float distanceToTargetScale = 1.0f)
        {
            Vector2 toTarget = ToTarget();
            float distanceFromTarget = toTarget.magnitude;

            Vector2 destination =
                GetTargetPosition() - GetTarget().forward * distanceFromTarget * distanceToTargetScale;
            Vector2 toDestination = destination - (Vector2) transform.position;

            toDestination = toDestination - toTarget.normalized / toTarget.sqrMagnitude * avoidance;
            toDestination = toDestination.sqrMagnitude > closeDist * closeDist ? toDestination : Vector2.zero;

            return toDestination;
        }

        public Vector2 StayBehindTarget(float closeDist = 2.0f, float avoidance = 600, float distanceToTarget = 10.0f)
        {
            Vector2 toTarget = ToTarget();
            Vector2 destination = GetTargetPosition() - GetTarget().forward * distanceToTarget;
            Vector2 toDestination = destination - (Vector2) transform.position;

            toDestination = toDestination - toTarget.normalized / toTarget.sqrMagnitude * avoidance;
            toDestination = toDestination.sqrMagnitude > closeDist * closeDist ? toDestination : Vector2.zero;

            return toDestination;
        }

        public Vector2 BehindTargetPosition()
        {
            Vector2 toTarget = ToTarget();
            float distanceFromTarget = toTarget.magnitude;

            return GetTargetPosition() - GetTarget().forward * distanceFromTarget;
        }

        #endregion UtilityFunctions
    }


    public class FocusStorage : FocusBase
    {
        public FocusStorage(Transform myTransform, StimuliFilter filter)
            : base(myTransform)
        {
            _filter = filter;
        }
        StimuliFilter _filter;

        public override MemoryEvent GetTarget()
        {
            return _filter.GetTarget();
        }
    }*/


}