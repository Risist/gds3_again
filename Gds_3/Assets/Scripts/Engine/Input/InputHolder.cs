using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds Input collected from Input recorders
/// Separation of input and input result soo ai, player, network agents can use the same body and actions
/// </summary>
public class InputHolder : MonoBehaviour
{
    public enum EInputVectorType
    {
        EPositionInput,
        EDirectionInput,
        ERotationInput
    }
    public Vector2 GetInputVector(EInputVectorType type)
    {
        switch(type)
        {
            case EInputVectorType.EPositionInput:
                return positionInput;
            case EInputVectorType.EDirectionInput:
                return directionInput;
            case EInputVectorType.ERotationInput:
                return rotationInput;
        }

        throw new System.NotImplementedException();
    }

    public bool HasInput(EInputVectorType type)
    {
        switch (type)
        {
            case EInputVectorType.EPositionInput:
                return atMove;
            case EInputVectorType.EDirectionInput:
                return atDirection;
            case EInputVectorType.ERotationInput:
                return atRotation;
        }

        throw new System.NotImplementedException();
    }
    public Vector2 positionInput;
    public Vector2 directionInput;
    public Vector2 rotationInput;

    public bool[] keys = new bool[4];

    [Range(0.0f, 1.0f), Tooltip("if input source has value lower that this value input is ignored")]
    public float inputThreshold = 0.25f;

    public bool atRotation  { get { return rotationInput .sqrMagnitude > inputThreshold * inputThreshold; } }
    public bool atMove      { get { return positionInput .sqrMagnitude > inputThreshold * inputThreshold; } }
    public bool atDirection { get { return directionInput.sqrMagnitude > inputThreshold * inputThreshold; } }

    public void ResetInput()
    {
        positionInput = Vector2.zero;
        directionInput = Vector2.zero;
        rotationInput = Vector2.zero;

        for (int i = 0; i < keys.Length; ++i)
            keys[i] = false;
    }
}
