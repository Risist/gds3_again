using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    [Serializable]
    public class PatrolPath
    {
        public enum EPatrolPathType
        {
            EOneOff,
            ELoop,
            EPingpong
        }
        public List<Vector3> pointList = new List<Vector3>();
        public EPatrolPathType pathType;

        public Vector3 GetClosestPoint(Vector3 point)
        {
            float minDistSq = float.MaxValue;
            Vector3 bestPoint = Vector3.zero;
            foreach(var it in pointList)
            {
                float distSq = (point - it).sqrMagnitude;
                if(distSq < minDistSq)
                {
                    minDistSq = distSq;
                    bestPoint = it;
                }
            }

            return bestPoint;
        }
        public int GetClosestPointId(Vector3 point)
        {
            float minDistSq = float.MaxValue;
            int bestPoint = -1;
            for(int i = 0; i < pointList.Count; ++i)
            {
                float distSq = (point - pointList[i]).sqrMagnitude;
                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    bestPoint = i;
                }
            }

            return bestPoint;
        }
    }

    public class PatrolPathFollower
    {
        public PatrolPathFollower(float nodeProximityDistance = 1.0f)
        {
            this.nodeProximityDistance = nodeProximityDistance;
        }

        public enum ETraversalDirection
        {
            EForward = 1,
            EBackward = -1,
            // randomly choses a direction
            ERandom = 0,
            // moves to the closer point
            EAuto = 2,
        }
        // called when traversal direction changes (or new loop starts in case of ELoop)
        public Action onPathFinished = () => { };

        // called when traversal direction changes (or new loop starts in case of ELoop)
        public Action onCurrentPointChanges = () => { };

        public float nodeProximityDistance;
        public PatrolPath path { get; private set; }
        public Vector3 CurrentPoint => path.pointList[_currentIndex];

        int _currentIndex;
        int _direction;

        public void StartPathFollowing(PatrolPath path, ETraversalDirection traversalDirection)
        {
            this.path = path; 
            if (traversalDirection == ETraversalDirection.ERandom)
            {
                traversalDirection = UnityEngine.Random.value > 0.5f ?
                    ETraversalDirection.EForward :
                    ETraversalDirection.EBackward;
            }
            _direction = (int)traversalDirection;

            
            _currentIndex = traversalDirection == ETraversalDirection.EForward ?
                0 :
                path.pointList.Count - 1;
        }

        public ETraversalDirection PreprocessTraversalDirection(Vector3 travelerPosition, PatrolPath path, ETraversalDirection traversalDirection, int currentIndex)
        {
            if(traversalDirection == ETraversalDirection.EForward || traversalDirection == ETraversalDirection.EBackward)
            {
                return traversalDirection;
            }

            if (currentIndex <= 0)
            {
                return ETraversalDirection.EForward;
            }
            
            if (currentIndex >= path.pointList.Count - 1)
            {
                return ETraversalDirection.EBackward;
            }

            
            
            if (traversalDirection == ETraversalDirection.EAuto)
            {
                Vector3 targetPoint = path.pointList[currentIndex];
                Vector3 targetPointLeft = path.pointList [currentIndex - 1];
                Vector3 targetPointRight = path.pointList[currentIndex + 1];

                float distSq = (targetPoint - travelerPosition).sqrMagnitude;
                float distSqLeft = (targetPointLeft - travelerPosition).sqrMagnitude;
                float distSqRight = (targetPointRight - travelerPosition).sqrMagnitude;

                if ( distSq >= nodeProximityDistance.Sq() )
                {
                    Vector3 closerPoint = distSqLeft >= distSqRight ? targetPointRight : targetPointLeft;
                    int closerIndex = distSqLeft >= distSqRight ? currentIndex + 1 : currentIndex - 1; 
                    float closerDistSq = Mathf.Min(distSqLeft, distSqRight);

                    if(distSq < closerDistSq)
                    {
                        return traversalDirection;
                    }else
                    {
                        return distSqLeft >= distSqRight ? ETraversalDirection.EBackward : ETraversalDirection.EForward;
                    }
                }
            }
            
            //else if(traversalDirection == ETraversalDirection.ERandom)
            {
                return UnityEngine.Random.value > 0.5f ?
                        ETraversalDirection.EForward :
                        ETraversalDirection.EBackward;
            }
        }
        public void StartPathFollowing(Vector3 travelerPosition, PatrolPath path, ETraversalDirection traversalDirection, int currentIndex)
        {
            this.path = path;
            traversalDirection = PreprocessTraversalDirection(travelerPosition, path, traversalDirection, currentIndex);
            
            _direction = (int)traversalDirection;
            _currentIndex = currentIndex;
        }
        public void AbortPathFollowing()
        {
            onPathFinished();
            path = null;
        }


        public void ChangeTraversalDirection()
        {
            switch (path.pathType)
            {
                case PatrolPath.EPatrolPathType.EOneOff:
                    break;
                case PatrolPath.EPatrolPathType.ELoop:
                    _currentIndex = 0;
                    _direction = -_direction;
                    break;
                case PatrolPath.EPatrolPathType.EPingpong:
                    _direction = -_direction;
                    break;
            }

            onPathFinished();
        }

        // updates following state
        // returns true if any change to the patrol point has happened
        public bool ProceedPathFollowing(Vector3 followerPosition)
        {
            float nextNodeDistSq = nodeProximityDistance.Sq();
            bool anyChange = false;
            while (true)
            {
                if (!RangedInt.InRange(_currentIndex, 0, path.pointList.Count))
                {
                    _currentIndex -= _direction;
                    ChangeTraversalDirection();
                    anyChange = true;
                    break;
                }

                Vector3 toDest = CurrentPoint - followerPosition;
                toDest = toDest.ToPlane();
                if (toDest.sqrMagnitude >= nextNodeDistSq)
                    break;

                onCurrentPointChanges();
                _currentIndex += _direction;
                anyChange = true;
            };

            return anyChange;
        }

    }

    //TODO Patrol path object to place on to the map

    //TODO Some manager for patrol paths so that you can search for them
}
