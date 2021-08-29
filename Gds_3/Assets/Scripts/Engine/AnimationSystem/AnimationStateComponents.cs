using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AnimationStateComponent
{
    public AnimationStateMachine stateMachine;
    public virtual void OnComponentAdd(AnimationState state) { }
    public virtual void OnComponentInit() { }
}

public class AState_RotationToDirection : AnimationStateComponent
{
    public AState_RotationToDirection(float lerpScale = 0.3f, float trackFactor = 1f)
    {
        this.rotationSpeed = lerpScale;
        this.trackFactor = trackFactor;
        this.applicationPeriod = new RangedFloat();
    }
    public AState_RotationToDirection(RangedFloat applicationPeriod, float lerpScale = 0.3f, float trackFactor = 1f)
    {
        this.rotationSpeed = lerpScale;
        this.trackFactor = trackFactor;
        this.applicationPeriod = applicationPeriod;
    }

    public RangedFloat applicationPeriod;
    public float rotationSpeed;
    public float trackFactor = 0f;
    public Vector2 destinationDirection { get; private set; }

    InputHolder _inputHolder;
    RigidbodyMovement _movement;

    public override void OnComponentInit()
    {
        _inputHolder = stateMachine.GetComponentInParent<InputHolder>();
        _movement = stateMachine.GetComponentInParent<RigidbodyMovement>();
    }
    public override void OnComponentAdd(AnimationState state)
    {
        state.AddOnBegin(Begin);
        state.AddOnEnd(End);
        state.AddFixedUpdate(FixedUpdate);
    }

    public void Begin()
    {
        Begin(_inputHolder.directionInput);
    }
    public void Begin(Vector2 destinationDirection)
    {
        this.destinationDirection = destinationDirection;
    }

    public void FixedUpdate(float animationTime)
    {
        FixedUpdate(animationTime, _inputHolder.directionInput);
    }
    public void FixedUpdate(float animationTime, Vector2 currentInput)
    {
        Vector2 destination = Vector2.Lerp(currentInput, destinationDirection, trackFactor);
        _movement.atExternalRotation = false;
        if (applicationPeriod.InRange(animationTime))
            _movement.ApplyExternalRotation(destination, rotationSpeed);
    }

    public void End()
    {
        _movement.atExternalRotation = false;
    }

}

public class AState_Motor : AnimationStateComponent
{
    public AState_Motor(Vector2 movementSpeed)
    {
        this.movementSpeed = movementSpeed;
        this.applicationPeriod = new RangedFloat();
    }
    public AState_Motor(Vector2 movementSpeed, RangedFloat applicationPeriod)
    {
        this.movementSpeed = movementSpeed;
        this.applicationPeriod = applicationPeriod;
    }
    public Vector2 movementSpeed;
    public RangedFloat applicationPeriod;
    Vector2 initialDir;

    InputHolder _inputHolder;
    Rigidbody _rigidbody;

    public override void OnComponentInit()
    {
        _inputHolder = stateMachine.GetComponentInParent<InputHolder>();
        _rigidbody = stateMachine.GetComponentInParent<Rigidbody>();
    }
    public override void OnComponentAdd(AnimationState state)
    {
        state.AddOnBegin(Begin);
        state.AddFixedUpdate(FixedUpdate);
    }

    public void Begin(Vector2 initialDir)
    {
        this.initialDir = initialDir.normalized;
    }

    public void Begin()
    {
        Begin(_inputHolder.directionInput);
    }
    public void FixedUpdate(float animationTime)
    {
        if (applicationPeriod.InRange(animationTime))
        {
            Vector2 force = initialDir * movementSpeed.y +
                new Vector2(initialDir.y, -initialDir.x) * movementSpeed.x;
            _rigidbody.AddForce(force.To3D(),ForceMode.Acceleration);
        }
    }
}

public class AState_JumpMotor : AnimationStateComponent
{
    public AState_JumpMotor(float movementSpeed, float minDotValue = -1f)
    {
        this.movementSpeed = new float[] { movementSpeed, movementSpeed, movementSpeed, movementSpeed };
        this.applicationPeriod = new RangedFloat();
        this.minDotValue = minDotValue;
    }
    public AState_JumpMotor(float movementSpeed, RangedFloat applicationPeriod, float minDotValue = -1f)
    {
        this.movementSpeed = new float[] { movementSpeed, movementSpeed, movementSpeed, movementSpeed };
        this.applicationPeriod = applicationPeriod;
        this.minDotValue = minDotValue;
    }
    public AState_JumpMotor(float[] movementSpeed, float minDotValue = -1f)
    {
        Debug.Assert(movementSpeed.Length <= 4);
        this.movementSpeed = movementSpeed;
        this.applicationPeriod = new RangedFloat();
        this.minDotValue = minDotValue;
    }
    public AState_JumpMotor(float[] movementSpeed, RangedFloat applicationPeriod, float minDotValue = -1f)
    {
        Debug.Assert(movementSpeed.Length <= 4);
        this.movementSpeed = movementSpeed;
        this.applicationPeriod = applicationPeriod;
        this.minDotValue = minDotValue;
    }

    public float[] movementSpeed;
    public RangedFloat applicationPeriod;
    public float minDotValue;
    public int defaultDirection = -1;
    Vector2 initialInputDirection;

    public int currentDirectionId { get; private set; }

    InputHolder _inputHolder;
    Rigidbody _rigidbody;

    public override void OnComponentInit()
    {
        _inputHolder = stateMachine.GetComponentInParent<InputHolder>();
        _rigidbody = stateMachine.GetComponentInParent<Rigidbody>();
    }
    public override void OnComponentAdd(AnimationState state)
    {
        state.AddOnBegin(Begin);
        state.AddFixedUpdate(FixedUpdate);
    }

    public AState_JumpMotor SetDefaultDirection(int d)
    {
        defaultDirection = d;
        return this;
    }

    public void Begin()
    {
        Vector2 directionInput = _inputHolder.directionInput.normalized;
        Vector2 positionInput = _inputHolder.positionInput.normalized;

        Vector2[] directions = {
                directionInput,
                -directionInput,
                new Vector2(directionInput.y, -directionInput.x),
                new Vector2(-directionInput.y, directionInput.x)
            };


        float maxDot = float.NegativeInfinity;
        initialInputDirection = defaultDirection == -1 ? Vector2.zero : directions[defaultDirection] * movementSpeed[defaultDirection];
        currentDirectionId = defaultDirection;
        if (!_inputHolder.atMove)
            return;

        for (int i = 0; i < movementSpeed.Length; ++i)
        {
            var newDot = Vector2.Dot(positionInput, directions[i]);
            if (newDot > minDotValue && newDot > maxDot)
            {
                maxDot = newDot;
                initialInputDirection = directions[i] * movementSpeed[i];
                currentDirectionId = i;
            }
        }
    }
    public void FixedUpdate(float animationTime)
    {
        if (applicationPeriod.InRange(animationTime))
        {
            _rigidbody.AddForce(initialInputDirection.To3D(), ForceMode.Acceleration);
        }
    }
}
