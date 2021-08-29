using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Ai
{
    public abstract class StateComponent
    {
        public BehaviourController behaviourController;
        public Transform transform => behaviourController.transform;
        public abstract void OnComponentAdd(State state);
    }

    public abstract class StateInputComponent : StateComponent
    {
        protected InputHolder inputHolder;

        public override void OnComponentAdd(State state)
        {
            inputHolder = behaviourController.GetComponentInParent<InputHolder>();
        }
    }

    public class LookAround
    {
        public LookAround(float rotationLerp = 0.1f)
        {
            this.rotationLerp = rotationLerp;
        }
        public LookAround(RangedFloat tChangeAngle, float rotationLerp = 0.1f)
        {
            this.rotationLerp = rotationLerp;
            this.tChangeAngle = tChangeAngle;
        }
        public LookAround(RangedFloat tChangeAngle, RangedFloat desiredAngleDifference, float rotationLerp = 0.1f)
        {
            this.rotationLerp = rotationLerp;
            this.tChangeAngle = tChangeAngle;
            this.desiredAngleDifference = desiredAngleDifference;
        }
        public void Init(Transform transform)
        {
            this.transform = transform;
        }

        public void SetState(State state, InputHolder inputHolder)
        {
            state.AddOnBegin(Begin);
            state.AddOnUpdate(() => inputHolder.rotationInput = UpdateRotationInput());
        }

        Transform transform;

        public RangedFloat desiredAngleDifference = new RangedFloat(45.0f, 120.0f);
        public RangedFloat tChangeAngle = new RangedFloat(1.0f, 1.0f);
        public float rotationLerp = 0.1f;
        public float chanceToChangeSide = 0.5f;

        readonly Timer _tChangeDesiredAngle = new Timer();
        float _desiredAngle;
        float _currentAngle;
        float _lastSide;

        public void SetNewDesiredAngle()
        {
            float diff = Random.Range(desiredAngleDifference.min, desiredAngleDifference.max);

            if (Random.value > chanceToChangeSide)
                _lastSide *= -1.0f;

            diff *= _lastSide;

            _desiredAngle = _currentAngle + diff;
        }

        public bool CloseEnoughToDediredAngle(float minAngleDifference = 0.0f)
        {
            float delta = Mathf.DeltaAngle(transform.eulerAngles.y, -_desiredAngle);
            return delta * delta < minAngleDifference * minAngleDifference;
        }

        public void Begin()
        {
            _lastSide = Random.value > 0.5f ? 1 : -1;
            SetNewDesiredAngle();
            _tChangeDesiredAngle.cd = Random.Range(tChangeAngle.min, tChangeAngle.max);
            _tChangeDesiredAngle.Restart();

            _currentAngle = transform.eulerAngles.y;
        }

        public Vector2 UpdateRotationInput()
        {
            if (_tChangeDesiredAngle.IsReadyRestart())
            {
                Begin();
            }

            _currentAngle = Mathf.LerpAngle(_currentAngle, _desiredAngle, rotationLerp);
            Vector2 desiredRotationInput = Quaternion.Euler(0, 0, -_currentAngle) * Vector3.up;
            return desiredRotationInput;
        }
    }

    public class LookAt
    {
        public LookAt(Transform transform, float colliderRadius = 0.5f)
        {
            this.colliderRadius = colliderRadius;
            this.transform = transform;
        }

        public float colliderRadius;
        public Transform transform;

        MemoryEvent _memoryEvent = new MemoryEvent();

        public void SetDestination(Vector3 destination, Vector3 velocity)
        {
            _memoryEvent.velocity = velocity;
            _memoryEvent.exactPosition = destination;
            _memoryEvent.lifetimeTimer.Restart();
        }
        public void SetDestination(Vector3 destination)
        {
            _memoryEvent.velocity = Vector3.zero;
            _memoryEvent.exactPosition = destination;
            _memoryEvent.lifetimeTimer.Restart();
        }
        public void SetDestination(MemoryEvent memoryEvent)
        {
            this._memoryEvent.velocity = memoryEvent.velocity;
            this._memoryEvent.exactPosition = memoryEvent.exactPosition;
            this._memoryEvent.lifetimeTimer.actualTime = memoryEvent.lifetimeTimer.actualTime;
        }

        public float GetDesiredAngle()
        {
            Vector2 dir = (transform.position - (Vector3)_memoryEvent.position);
            Vector2 forward = transform.forward.To2D();
            return Vector2.SignedAngle(dir, forward);
        }

        public Vector2 UpdateRotationInput()
        {
            Vector3 currentPosition = _memoryEvent.position;
            if (CollisionCheck(transform.position.To2D(), colliderRadius, _memoryEvent.exactPosition, _memoryEvent.velocity))
                currentPosition = _memoryEvent.exactPosition;

            Vector2 dir = currentPosition.To2D() - transform.position.To2D();
            return dir;
        }

        public bool CloseEnoughToDediredAngle(float minAngleDifference = 0.0f)
        {
            float delta = Mathf.DeltaAngle(transform.eulerAngles.y, -GetDesiredAngle());
            return delta * delta < minAngleDifference * minAngleDifference;
        }


        bool CollisionCheck(Vector2 circlePosition, float circleRadius, Vector2 linePosition, Vector2 velocity)
        {
            Vector2 vNorm = velocity.normalized;
            Vector2 diff = circlePosition - linePosition;
            float dot = Vector2.Dot(vNorm, diff);
            Vector2 d = linePosition + vNorm * dot;

            return
                // line collision
                (d - circlePosition).sqrMagnitude < circleRadius * circleRadius &&
                // segment collision
                dot * dot < diff.sqrMagnitude;
        }
    }


    public class WalkRandomly
    {
        public WalkRandomly(RangedFloat walkDistance)
        {
            this.walkDistance = walkDistance;
        }
        
        public void Init(Transform transform)
        {
            this.transform = transform;
            currentDestination = transform.position;
        }

        Transform transform;

        public RangedFloat walkDistance = new RangedFloat( 3.0f, 10.0f);

        public Vector3 currentDestination;

        public Vector3 SelectNewPosition()
        {
            Vector2 offset = Random.insideUnitSphere * walkDistance.GetRandom();
            currentDestination = transform.position + offset.To3D();
            return currentDestination;
        }

        public Vector3 SelectNewPosition(Vector3 mixWith, float factor)
        {
            Vector3 center = Vector3.Lerp(transform.position, mixWith, factor);
            Vector2 offset = Random.insideUnitSphere * walkDistance.GetRandom();
            currentDestination = center + offset.To3D();
            return currentDestination;
        }

        public bool IsCloseToDestination( float dist )
        {
            Vector3 toDestination = currentDestination - transform.position;
            return toDestination.To2D().sqrMagnitude < dist* dist;
        }
    }

    /*
    public class ComponentLookAround : StateInputComponent
    {
        public ComponentLookAround(float rotationLerp = 0.1f)
        {
            this.rotationLerp = rotationLerp;
        }
        public ComponentLookAround(RangedFloat tChangeAngle, float rotationLerp = 0.1f)
        {
            this.rotationLerp = rotationLerp;
            this.tChangeAngle = tChangeAngle;
        }
        public ComponentLookAround(RangedFloat tChangeAngle, RangedFloat desiredAngleDifference, float rotationLerp = 0.1f)
        {
            this.rotationLerp = rotationLerp;
            this.tChangeAngle = tChangeAngle;
            this.desiredAngleDifference = desiredAngleDifference;
        }

        public override void OnComponentAdd(State state)
        {
            base.OnComponentAdd(state);
            state.AddOnBegin(Begin);
            state.AddOnUpdate(() => inputHolder.rotationInput = UpdateRotationInput() );
        }

        public RangedFloat desiredAngleDifference = new RangedFloat(45.0f, 120.0f);
        public RangedFloat tChangeAngle = new RangedFloat(1.0f, 1.0f);
        public float rotationLerp = 0.1f;

        readonly Timer _tChangeDesiredAngle = new Timer();
        float _desiredAngle;
        float _currentAngle;

        public void SetNewDesiredAngle()
        {
            float diff = Random.Range(desiredAngleDifference.min, desiredAngleDifference.max);
            diff = Random.value > 0.5f ? diff : -diff;

            _desiredAngle = _currentAngle + diff;
        }

        public bool CloseEnoughToDediredAngle(float minAngleDifference = 0.0f)
        {
            float delta = Mathf.DeltaAngle(transform.eulerAngles.y, -_desiredAngle);
            return delta * delta < minAngleDifference * minAngleDifference;
        }

        public void Begin()
        {
            SetNewDesiredAngle();
            _tChangeDesiredAngle.cd = Random.Range(tChangeAngle.min, tChangeAngle.max);
            _tChangeDesiredAngle.Restart();

            _currentAngle = transform.eulerAngles.y;
        }

        public Vector2 UpdateRotationInput()
        {
            if (_tChangeDesiredAngle.IsReadyRestart())
            {
                Begin();
            }

            _currentAngle = Mathf.LerpAngle(_currentAngle, _desiredAngle, rotationLerp);
            Vector2 desiredRotationInput = Quaternion.Euler(0, 0, -_currentAngle) * Vector3.up;
            return desiredRotationInput;
        }
    }*/

    /*public class ComponentLookAt : StateInputComponent
    {
        public ComponentLookAt(float colliderRadius = 0.5f)
        {
            this.colliderRadius = colliderRadius;
        }

        public override void OnComponentAdd(State state)
        {
            base.OnComponentAdd(state);
        }

        public float colliderRadius;

        MemoryEvent _memoryEvent = new MemoryEvent();

        public void SetDestination(Vector3 destination, Vector3 velocity)
        {
            _memoryEvent.velocity = velocity;
            _memoryEvent.exactPosition = destination;
            _memoryEvent.lifetimeTimer.Restart();
        }
        public void SetDestination(Vector3 destination)
        {
            _memoryEvent.velocity = Vector3.zero;
            _memoryEvent.exactPosition = destination;
            _memoryEvent.lifetimeTimer.Restart();
        }
        public void SetDestination(MemoryEvent memoryEvent)
        {
            this._memoryEvent.velocity = memoryEvent.velocity;
            this._memoryEvent.exactPosition = memoryEvent.exactPosition;
            this._memoryEvent.lifetimeTimer.actualTime = memoryEvent.lifetimeTimer.actualTime;
        }

        public float GetDesiredAngle()
        {
            Vector2 dir = (transform.position - (Vector3)_memoryEvent.position);
            Vector2 forward = transform.forward.To2D();
            return Vector2.SignedAngle(dir, forward);
        }

        public Vector2 UpdateRotationInput()
        {
            Vector3 currentPosition = _memoryEvent.position;
            if (CollisionCheck(transform.position.To2D(), colliderRadius, _memoryEvent.exactPosition, _memoryEvent.velocity))
                currentPosition = _memoryEvent.exactPosition;

            Vector2 dir = currentPosition.To2D() - transform.position.To2D();
            return dir;
        }

        public bool CloseEnoughToDediredAngle(float minAngleDifference = 0.0f)
        {
            float delta = Mathf.DeltaAngle(transform.eulerAngles.y, -GetDesiredAngle());
            return delta * delta < minAngleDifference * minAngleDifference;
        }


        bool CollisionCheck(Vector2 circlePosition, float circleRadius, Vector2 linePosition, Vector2 velocity)
        {
            Vector2 vNorm = velocity.normalized;
            Vector2 diff = circlePosition - linePosition;
            float dot = Vector2.Dot(vNorm, diff);
            Vector2 d = linePosition + vNorm * dot;

            return
                // line collision
                (d - circlePosition).sqrMagnitude < circleRadius * circleRadius &&
                // segment collision
                dot * dot < diff.sqrMagnitude;
        }

        
    }*/

    // current angle + rotation around
    // look at + rotation arround

    /*public class LookAround : StateComponent
    {
        public LookAround(Transform transform)
        {
            this.transform = transform;
        }
        public LookAround(Transform transform, RangedFloat tChangeAngle)
        {
            this.transform = transform;
            this.tChangeAngle = tChangeAngle;
        }
        public LookAround(Transform transform, RangedFloat tChangeAngle, RangedFloat desiredAngleDifference)
        {
            this.transform = transform;
            this.tChangeAngle = tChangeAngle;
            this.desiredAngleDifference = desiredAngleDifference;
        }

        Transform transform;
        public RangedFloat desiredAngleDifference = new RangedFloat(45.0f, 120.0f);
        public RangedFloat tChangeAngle = new RangedFloat(1.0f, 1.0f);

        readonly Timer _tChangeDesiredAngle = new Timer();
        float _desiredAngle;
        float _currentAngle;

        public void Begin()
        {
            SetNewDesiredAngle();
            _tChangeDesiredAngle.cd = Random.Range(tChangeAngle.min, tChangeAngle.max);
            _tChangeDesiredAngle.Restart();

            transform = behaviourController.transform;
            _currentAngle = transform.eulerAngles.y;
        }

        public void SetNewDesiredAngle()
        {
            float diff = Random.Range(desiredAngleDifference.min, desiredAngleDifference.max);
            diff = Random.value > 0.5f ? diff : -diff;

            _desiredAngle = _currentAngle + diff;
        }
        public Vector2 UpdateRotationInput(float rotationLerp = 0.1f)
        {
            if (_tChangeDesiredAngle.IsReadyRestart())
            {
                Begin();
            }

            _currentAngle = Mathf.LerpAngle(_currentAngle, _desiredAngle, rotationLerp);
            Vector2 desiredRotationInput = Quaternion.Euler(0, 0, -_currentAngle) * Vector3.up;
            return desiredRotationInput;
        }

        public bool CloseEnoughToDediredAngle( float minAngleDifference = 0.0f)
        {
            float delta = Mathf.DeltaAngle(transform.eulerAngles.y, -_desiredAngle);
            return delta * delta < minAngleDifference * minAngleDifference;
        }

        public override void OnComponentAdd(State state)
        {
            state.AddOnBegin(Begin);
        }
    }
    public class LookAt : StateComponent
    {
        public LookAt(Transform transform, float colliderRadius = 0.5f)
        {
            this.colliderRadius = colliderRadius;
            this.transform = transform;
        }

        public float colliderRadius;
        public Transform transform;

        MemoryEvent _memoryEvent = new MemoryEvent();

        public void SetDestination(Vector3 destination, Vector3 velocity)
        {
            _memoryEvent.velocity = velocity;
            _memoryEvent.exactPosition = destination;
            _memoryEvent.lifetimeTimer.Restart();
        }
        public void SetDestination(Vector3 destination)
        {
            _memoryEvent.velocity = Vector3.zero;
            _memoryEvent.exactPosition = destination;
            _memoryEvent.lifetimeTimer.Restart();
        }
        public void SetDestination(MemoryEvent memoryEvent)
        {
            this._memoryEvent.velocity = memoryEvent.velocity;
            this._memoryEvent.exactPosition = memoryEvent.exactPosition;
            this._memoryEvent.lifetimeTimer.actualTime = memoryEvent.lifetimeTimer.actualTime;
        }

        public float GetDesiredAngle()
        {
            Vector2 dir = (transform.position - (Vector3)_memoryEvent.position);
            Vector2 forward = transform.forward.To2D();
            return Vector2.SignedAngle(dir, forward);
        }

        public Vector2 UpdateRotationInput()
        {
            Vector3 currentPosition = _memoryEvent.position;
            if (CollisionCheck(transform.position.To2D(), colliderRadius, _memoryEvent.exactPosition, _memoryEvent.velocity))
                currentPosition = _memoryEvent.exactPosition;

            Vector2 dir = currentPosition.To2D() - transform.position.To2D();
            return dir;
        }

        public bool CloseEnoughToDediredAngle(float minAngleDifference = 0.0f)
        {
            float delta = Mathf.DeltaAngle(transform.eulerAngles.y, -GetDesiredAngle());
            return delta * delta < minAngleDifference * minAngleDifference;
        }


        bool CollisionCheck(Vector2 circlePosition, float circleRadius, Vector2 linePosition, Vector2 velocity)
        {
            Vector2 vNorm = velocity.normalized;
            Vector2 diff = circlePosition - linePosition;
            float dot = Vector2.Dot(vNorm, diff);
            Vector2 d = linePosition + vNorm * dot;

            return
                // line collision
                (d - circlePosition).sqrMagnitude < circleRadius * circleRadius &&
                // segment collision
                dot * dot < diff.sqrMagnitude;
        }

        public override void OnComponentAdd(State state)
        {

        }
    }
    */

    public class MoveToDestination
    {
        public MoveToDestination(Transform transform)
        {
            this.transform = transform;
        }

        public Transform transform { get; private set; }
        public Vector3 destination { get; private set; }
        public Vector3 toDestination { get { return destination - transform.position; } }

        #region New Target
        /// moves to destination in area around target (max in search area)
        /// if target is null treat yourself as target
        /// target position is predicted in future, You can adjust the prediction via @maxRespectedTime and @timeScale 
        /// @maxRespectedTime -> clamps how much in future the event will be predicted
        /// @timeScale -> specifies how important prediction is 
        /*public void SetDestination_Search(FocusBase focus, float searchArea, float maxRespectedTime = float.MaxValue, float timeScale = 1.0f)
        {
            if (!focus.HasTarget())
            {
                destination = (Vector2)transform.position + Random.insideUnitCircle * searchArea;
                return;
            }

            destination = focus.GetTarget().GetPosition(maxRespectedTime, timeScale) + Random.insideUnitCircle * searchArea;
        }
        public void SetDestination_Flee(FocusBase focus, RangedFloat desiredDistance, float searchArea = 1.0f)
        {
            if (!focus.HasTarget())
            {
                destination = (Vector2)transform.position + Random.insideUnitCircle.normalized * desiredDistance.GetRandom() + Random.insideUnitCircle * searchArea;
                return;
            }

            Vector2 toTarget = -focus.ToTarget();
            destination = (Vector2)transform.position + toTarget.normalized * desiredDistance.GetRandom() + Random.insideUnitCircle * searchArea;
        }*/
        public void SetDestination(Vector3 d)
        {
            destination = d;
        }


        #endregion New Target

        public Vector2 ToDestination(float closeDistance = 1.5f)
        {
            return IsCloseToDestination(closeDistance) ? Vector2.zero : toDestination.To2D();
        }

        public bool IsCloseToDestination(float closeDistance)
        {
            return toDestination.To2D().sqrMagnitude < closeDistance * closeDistance;
        }
    }
    public class MoveToDestinationNavigation : MoveToDestination
    {
        public MoveToDestinationNavigation(Seeker seeker) : base(seeker.transform)
        {
            this.seeker = seeker;
        }

        public Seeker seeker { get; private set; }
        public Path path { get; private set; }
        public int nodeId { get; private set; }
        public bool Finished { get => path != null && nodeId >= path.vectorPath.Count; }

        public Timer tRaycastCheck = new Timer(0.25f);
        


        #region New Target
        /// moves to destination in area around target (max in search area)
        /// if target is null treat yourself as target
        /// target position is predicted in future, You can adjust the prediction via @maxRespectedTime and @timeScale 
        /// @maxRespectedTime -> clamps how much in future the event will be predicted
        /// @timeScale -> specifies how important prediction is 
        /*new public void SetDestination_Search(FocusBase focus,float searchArea, float maxRespectedTime = float.MaxValue, float timeScale = 1.0f)
        {
            base.SetDestination_Search(focus, searchArea, maxRespectedTime, timeScale);
            SetDestination(destination);
        }
        new public void SetDestination_Flee(FocusBase focus, RangedFloat desiredDistance, float searchArea = 1.0f)
        {
            base.SetDestination_Flee(focus, desiredDistance, searchArea);
            SetDestination(destination);
        }*/
        new public void SetDestination(Vector3 d)
        {
            base.SetDestination(d);
            seeker.detailedGizmos = true;
            var nearest = PathfindingManager.Instance.path.GetNearest(destination).position;
            seeker.StartPath(transform.position, nearest, (Path p) => { path = p; });
            nodeId = 0;
        }


        #endregion New Target
        public void RepathAsNeeded(Vector3 currentTargetPosition, float maxDistanceFromDestination)
        {
            Vector2 toDestination = currentTargetPosition - destination;
            if ( toDestination.sqrMagnitude >= maxDistanceFromDestination.Sq() || Finished)
                SetDestination(currentTargetPosition);
        }


        public Vector2 ToDestinationLerp(Vector3 currentTargetPosition, float correctionScale = 0.5f, float closeDist = 1.5f, float nextNodeDist = 0.5f)
        {
            Vector2 toDestination = ToDestination(closeDist, nextNodeDist);
            if (toDestination == Vector2.zero)
                return Vector2.zero;

            return Vector2.Lerp(toDestination, (currentTargetPosition - transform.position).To2D(), correctionScale);
        }
        public Vector2 ToDestination(float closeDist = 1.5f, float nextNodeDist = 0.5f)
        {
            if (IsCloseToDestination(closeDist))
                return Vector2.zero;

            if (path != null && path.IsDone())
            {
                Vector3 toDest = Vector3.zero;
                float nextNodeDistSq = nextNodeDist.Sq();
                while (nodeId < path.vectorPath.Count)
                {
                    toDest = path.vectorPath[nodeId] - transform.position;
                    toDest = toDest.ToPlane();
                    if (toDest.sqrMagnitude >= nextNodeDistSq)
                        break;

                    ++nodeId;
                };

                //Debug.DrawRay(path.vectorPath[nodeId], Vector2.up, Color.yellow, 0.05f);
                return toDest.To2D();
            }

            return toDestination;
        }

        public Vector2 ToDestinationAdaptiveLerp(Vector3 currentTargetPosition, float correctionScale = 0.5f, float closeDistance = 1.5f, float nextNodeDist = 0.5f, float maxDistanceFromDestinationToRepath = 1.5f)
        {
            if( (currentTargetPosition - transform.position).To2D().sqrMagnitude < closeDistance.Sq() )
                return Vector2.zero;

            Vector2 toDestination = ToDestinationAdaptive(
                currentTargetPosition,
                0,
                nextNodeDist,
                maxDistanceFromDestinationToRepath
            );

            return Vector2.Lerp(toDestination, (currentTargetPosition - transform.position).To2D(), correctionScale);
        }
        public Vector2 ToDestinationAdaptive(Vector3 currentTargetPosition, float closeDistance = 1.5f, float nextNodeDist = 0.5f, float maxDistanceFromDestinationToRepath = 1.5f )
        {
            if (IsPathFree(currentTargetPosition))
            {
                ClearPath();
                base.SetDestination(currentTargetPosition);
                return base.ToDestination(closeDistance);
            }
            else
            {
                RepathAsNeeded(currentTargetPosition, maxDistanceFromDestinationToRepath);
                return ToDestination(closeDistance, nextNodeDist);
            }
        }

        public void ClearPath()
        {
            path = null;
        }

        public bool IsPathFree(Vector3 destination)
        {
            Vector3 toDestination = destination - transform.position;
            bool bHit = Physics.CheckBox(transform.position, new Vector3(1f, 2, toDestination.magnitude), Quaternion.LookRotation(toDestination), SightManager.Instance.obstacleMask);

            Debug.DrawRay(transform.position, toDestination, bHit ? Color.black : Color.white);
            return !bHit;
        }
    }
    // TODO character state detection
}
