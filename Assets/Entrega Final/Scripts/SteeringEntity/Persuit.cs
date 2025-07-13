using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Persuit : ISteering
{
    private Rigidbody rb;
    private Rigidbody targetRB;
    private float maxVelocity;
    private float timePrediction = 1;

    public Persuit(Rigidbody rb, Rigidbody target, float maxVelocity, float timePrediction)
    {
        this.rb = rb;
        this.targetRB = target;
        this.maxVelocity = maxVelocity;
        this.timePrediction = timePrediction;
    }

    private bool overrideTarget = false;
    public bool IsOverridingTarget => overrideTarget;

    private Vector3 targetPosition;
    public Vector3 TargetPosition => targetPosition;

    public Vector3 MoveDirection()
    {
        Vector3 destination;

        if (overrideTarget)
        {
            destination = targetPosition;
        }
        else
        {
            destination = targetRB.position + targetRB.velocity * timePrediction;
        }

        Vector3 desiredVelocity = (destination - rb.position).normalized * maxVelocity;
        Vector3 directionForce = desiredVelocity - rb.velocity;
        directionForce.y = 0;
        directionForce = Vector3.ClampMagnitude(directionForce, maxVelocity);

        return directionForce;
    }

    public void OverrideTarget(Vector3 newTargetPosition)
    {
        overrideTarget = true;
        targetPosition = newTargetPosition;
    }

    public void ResetTarget()
    {
        overrideTarget = false;
    }
}
