using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

public class ApplyInputOffsetToCamera : MonoBehaviour
{
    [Tooltip("If not set up will search for it in parent")]
    public InputHolder inputHolder;

    public InputHolder.EInputVectorType inputType;

    public float multiplayer = 1.0f;
    public float movementSpeed = 1.0f;

    [HideIf("normalize")]
    public float maxOutValue = float.MaxValue;

    [HideIf("normalize")]
    public float inputDeadzone = 0;

    public bool normalize = false;

    public bool applyAspectRatio;

    CameraController _cameraController;

    Vector2 lastOffset;


    private void OnDestroy()
    {
        var camera = Camera.main;
        camera?.GetComponent<CameraController>()?.RemoveDirectionOffset(GetDirectionOffset);
    }

    private void Start()
    {
        var camera = Camera.main;
        _cameraController = camera.GetComponent<CameraController>();

        if (!inputHolder)
        {
            inputHolder = GetComponentInParent<InputHolder>();
        }
        _cameraController = camera.GetComponent<CameraController>();
        _cameraController.AddDirectionOffset(GetDirectionOffset);
    }



    Vector3 GetDirectionOffset()
    {
        Vector2 currentInput = inputHolder.GetInputVector(inputType);

        float magnitude = currentInput.magnitude;
        Vector2 normalizedInput = currentInput.normalized;

        float finalMagnitude = normalize ? multiplayer : Mathf.Clamp(magnitude * multiplayer - inputDeadzone, 0, maxOutValue);

        if (!inputHolder.HasInput(inputType) || finalMagnitude < inputDeadzone)
        {
            finalMagnitude = 0;
        }

        if (applyAspectRatio)
        {
            normalizedInput.x /= Camera.main.aspect;
        }

        lastOffset = Vector2.Lerp(lastOffset, normalizedInput*finalMagnitude, movementSpeed * Time.deltaTime);

        return lastOffset.To3D();
    }



}
