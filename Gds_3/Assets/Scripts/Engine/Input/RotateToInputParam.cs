using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

public class RotateToInputParam : MonoBehaviour
{
    public enum ERotationSelectionTime
    {
        EAtUpdate,
        EOnEnable
    }
    public ERotationSelectionTime rotationSelectionTime;
    [Space]

    [InfoBox("Child objects MeshRenderers will have alpha value changed if no input")]
    [Tooltip("If not set up will search for it in parent")]
    public InputHolder inputHolder;
    public InputHolder.EInputVectorType inputType;
    public float maxSpeed = 250.0f;
    public float inputDeadzone = 0.0f;

    public bool updateRenderers = true;
    
    [ShowIf("updateRenderers")]
    public float alphaMaxSpeed = 100.0f;
    
    [ShowIf("updateRenderers")]
    public float idleAlpha = 1.0f;

    MeshRenderer[] meshRenderers;


    private void Awake()
    {
        if(!inputHolder)
        {
            inputHolder = GetComponentInParent<InputHolder>();
        }

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    private void OnEnable()
    {
        cachedInput = inputHolder.GetInputVector(inputType);
        cachedHasInput = inputHolder.HasInput(inputType);
    }

    Vector2 cachedInput;
    Vector2 inputVectorValue => rotationSelectionTime == ERotationSelectionTime.EAtUpdate ? inputHolder.GetInputVector(inputType) : cachedInput;
    bool cachedHasInput;
    bool hasInputValue => rotationSelectionTime == ERotationSelectionTime.EAtUpdate ? inputHolder.HasInput(inputType) : cachedHasInput;


    private void LateUpdate()
    {

        bool hasInput = hasInputValue && inputVectorValue.sqrMagnitude >= inputDeadzone.Sq();

        if (updateRenderers)
        {
            // color
            foreach (var it in meshRenderers)
            {
                var color = it.material.color;
                float desiredA = hasInput ? idleAlpha : 0.0f;
                color.a = Mathf.Lerp(color.a, desiredA, alphaMaxSpeed * Time.deltaTime);
                //color.a = color.a > 0.1f ? color.a : 0.0f;
                it.material.color = color;
            }
        }


        Vector2 currentVector = transform.forward.To2D();
        float currentAngle = Vector2.SignedAngle(currentVector, Vector2.up);

        if (hasInput)
        {
            // rotation
            Vector2 inputVector = inputVectorValue;
            float angle = Vector2.SignedAngle(inputVector, Vector2.up);

            float newAngle = Mathf.LerpAngle(currentAngle, angle, maxSpeed * Time.deltaTime);

            transform.eulerAngles = new Vector3(0, newAngle, 0);
        }else
        {
            // to ensure constant local rotation
            transform.eulerAngles = new Vector3(0, currentAngle, 0);
        }

    }

}
