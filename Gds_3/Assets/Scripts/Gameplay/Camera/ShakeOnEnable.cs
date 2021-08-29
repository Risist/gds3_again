using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;


public class ShakeOnEnable : MonoBehaviour
{
    public enum EMode
    {
        ERandomDirection,
        EForward,
        EInput
    }
    bool mode_RandomDirection => mode == EMode.ERandomDirection;
    bool mode_Forward => mode == EMode.EForward;
    bool mode_Input => mode == EMode.EInput;

    public EMode mode;
    public RangedFloat force;


    [ShowIf(EConditionOperator.Or, new string[] { "mode_Forward", "mode_Input" } )]
    public RangedFloat forceAngleOffset;

    [ShowIf("mode_Forward"), Tooltip("If not set up will take transform of this object")]
    public Transform directionTransform;

    [ShowIf("mode_Input")]
    public InputHolder.EInputVectorType inputType;

    [ShowIf("mode_Input"), Tooltip("If not set up will search for it in parent")]
    public InputHolder inputHolder;
    
    
    CameraController _controller;




    private void Awake()
    {
        _controller = Camera.main?.GetComponent<CameraController>();


        if (!inputHolder)
        {
            inputHolder = GetComponentInParent<InputHolder>();
        }

        if (!directionTransform)
        {
            directionTransform = transform;
        }
    }

    private void OnEnable()
    {
        if (mode_RandomDirection)
        {
            _controller.ApplyShakeRandom(force.GetRandom());
        }
        else if(mode_Forward)
        {
            Vector3 forward = directionTransform.forward;
            Vector3 forward2D = forward.To2D();

            float angle = Vector2.SignedAngle(forward2D, Vector3.up);

            angle += forceAngleOffset.GetRandomSigned();

            _controller.ApplyShake(Quaternion.Euler(0, angle, 0) * Vector3.forward * force.GetRandom()  );
        }
        else if (mode_Input)
        {
            Vector3 forward = inputHolder.GetInputVector(inputType);
            Vector3 forward2D = forward.To2D();

            float angle = Vector2.SignedAngle(forward2D, Vector3.up);

            angle += forceAngleOffset.GetRandomSigned();

            _controller.ApplyShake(Quaternion.Euler(0, angle, 0) * Vector3.forward * force.GetRandom());
        }
    }
}
