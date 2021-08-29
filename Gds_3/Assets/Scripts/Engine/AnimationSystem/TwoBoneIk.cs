using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TwoBoneIk : MonoBehaviour
{
    [Tooltip("Ik will try to touch the point")]
    public Transform target;
    [Tooltip("angle to which elbow will move")]
    public float elbowAngle;

    [Header("FirstBone")]
    public Transform firstBone;
    [Tooltip("Length of the first bone")]
    public float firstBoneLength;


    [Header("SecondBone")]
    public Transform secondBone;
    [Tooltip("Length of the second bone")]
    public float secondBoneLength;



    new Transform transform;

    void Start()
    {
        transform = base.transform;
    }

    void Update()
    {
        if (!target)
            return;

        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;
        toTarget /= distance;

        if (distance > firstBoneLength + secondBoneLength)
        {
            if (firstBone)
            {
                firstBone.LookAt(target);
                firstBone.position = transform.position + toTarget * firstBoneLength * 0.5f;

                if (secondBone)
                {
                    toTarget = firstBone.position - transform.position;
                    toTarget.Normalize();

                    secondBone.position = firstBone.position + toTarget * secondBoneLength;
                    secondBone.LookAt(target);
                }
            }
        }
        else
        {
            float p = (firstBoneLength * firstBoneLength - secondBoneLength * secondBoneLength) / (2 * distance * distance) + 0.5f;
            float pt = distance * p;
            float h = Mathf.Sqrt(firstBoneLength * firstBoneLength - pt * pt);


            Vector3 perpendicular = Vector3.Cross(toTarget, Quaternion.AngleAxis(elbowAngle, toTarget) * Vector3.up);
            Vector3 middle = toTarget * distance * p;
            Vector3 t = transform.position + middle + perpendicular * h;
            toTarget = t - transform.position;

            firstBone.position = transform.position + toTarget.normalized * firstBoneLength * 0.5f;
            firstBone.LookAt(t);

            if (secondBone)
            {
                toTarget = target.position - firstBone.position - firstBone.forward * firstBoneLength * 0.5f;
                toTarget.Normalize();


                secondBone.position = firstBone.position + firstBone.forward * firstBoneLength * 0.5f + Quaternion.LookRotation(toTarget) * Vector3.forward * secondBoneLength * 0.5f;
                secondBone.LookAt(target);
            }

        }
    }

    /*private void OnDrawGizmosSelected()
    {
        if (!target)
            return;

        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;
        toTarget /= distance;

        if (distance > firstBoneLength + secondBoneLength)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
        else
        {
            float p = (firstBoneLength * firstBoneLength - secondBoneLength * secondBoneLength) / (2 * distance * distance) + 0.5f;
            float pt = distance * p;
            float h = Mathf.Sqrt(firstBoneLength * firstBoneLength - pt * pt);


            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + toTarget * distance * p, 0.5f);


            Vector3 perpendicular = Vector3.Cross(toTarget, Quaternion.AngleAxis(elbowAngle, toTarget) * Vector3.up);
            Vector3 middle = toTarget * distance * p;
            Vector3 t = transform.position + middle + perpendicular * h;
            toTarget = t - transform.position;



            Gizmos.color = Color.red;
            Gizmos.DrawSphere(t, 0.5f);

        }
    }*/
}
