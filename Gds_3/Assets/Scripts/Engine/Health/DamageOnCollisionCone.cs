using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCollisionCone : DamageOnCollision
{
    public float coneAngle = 70.0f;
    protected bool IsRightAngle(Vector3 targetPosition)
    {
        //// check if the target is in proper angle
        Vector2 toIt = (targetPosition - transform.position).To2D();
        float cosAngle = Vector2.Dot(toIt.normalized, transform.forward.To2D());
        float angle = Mathf.Acos(cosAngle) * 180 / Mathf.PI;
        //Debug.Log(angle);
        bool bProperAngle = angle < coneAngle * 0.5f;
        return bProperAngle;
    }

    private new void OnTriggerStay(Collider other)
    {
        if (!IsRightAngle(other.transform.position))
            return;
        base.OnTriggerStay(other);
    }
    private new void OnTriggerEnter(Collider other)
    {
        if (!IsRightAngle(other.transform.position))
            return;
        base.OnTriggerEnter(other);
    }

    /*private new void OnCollisionStay(Collision collision)
    {
        if (!IsRightAngle(collision.transform.position))
            return;
        base.OnCollisionStay(collision);
    }
    private new void OnCollisionEnter(Collision collision)
    {
        if (!IsRightAngle(collision.transform.position))
            return;
        base.OnCollisionEnter(collision);
    }*/
}
