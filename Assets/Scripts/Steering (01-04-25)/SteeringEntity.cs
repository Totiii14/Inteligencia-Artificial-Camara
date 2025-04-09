using UnityEngine;

public class SteeringEntity : MonoBehaviour
{
    [SerializeField] Rigidbody target;
    [SerializeField] float maxVelocity;
    [SerializeField] float timePrediction;

    public SteeringMode mode;
    private ISteering currentSteering;

    Vector3 steeringVelocity;

    public Vector3 SteeringVelocity { get => steeringVelocity; set => steeringVelocity = value; }

    Rigidbody rb;

    ObstacleAvoid obs;
    //ExampleTeacher obstacleAv;

    public enum SteeringMode
    {
        seek,
        flee,
        persuit,
        evade
    }

    void Start()
    {
        //obstacleAv = GetComponent<ExampleTeacher>();
        obs = GetComponent<ObstacleAvoid>();
        rb = GetComponent<Rigidbody>();
        Flee flee = new(rb, target.transform, maxVelocity);
        Seek seek = new(rb, target.transform, maxVelocity);
        Persuit persuit = new(rb, target, maxVelocity, timePrediction);
        Evade evade = new(rb, target, maxVelocity, timePrediction);

        switch (mode)
        {
            case SteeringMode.seek:
                currentSteering = seek; 
                break;
            case SteeringMode.flee:
                currentSteering = flee;
                break;
            case SteeringMode.persuit:
                currentSteering = persuit;
                break;
            case SteeringMode.evade:
                currentSteering = evade;
                break;
        }
    }

    void Update()
    {
        if (obs.IsObstacle == false)
        {
            steeringVelocity = currentSteering.MoveDirection();
            //steeringVelocity += obstacleAv.Avoid() * steeringVelocity.magnitude;
            rb.AddForce(steeringVelocity, ForceMode.Acceleration);
            transform.forward = rb.velocity;
        }
        else
        {
            rb.velocity = steeringVelocity.normalized * maxVelocity;
            transform.forward = steeringVelocity.normalized;
            //transform.forward = rb.velocity;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(rb.position, rb.position + rb.velocity * 3);

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(rb.position, rb.position + desiredVelocity);

    //    Gizmos.DrawWireSphere(target.position + target.velocity * timePrediction * Vector3.Distance(rb.position, target.position) * 0.1f, 2);
    //}
}
