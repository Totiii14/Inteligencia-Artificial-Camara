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

    public Vector3 MoveDirection()
    {
        Vector3 futurePosition = targetRB.position + targetRB.velocity * timePrediction * Vector3.Distance(targetRB.position, rb.position);

        Vector3 desiredVelocity = (futurePosition - rb.position).normalized * maxVelocity;
        Vector3 directionForce = desiredVelocity - rb.velocity;
        directionForce.y = 0;
        directionForce = Vector3.ClampMagnitude(directionForce, maxVelocity);
        rb.AddForce(directionForce, ForceMode.Acceleration);

        return directionForce;
    }
}
