using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SteeringEntity : MonoBehaviour
{
    [SerializeField] Transform Target;
    [SerializeField] Rigidbody targetRb;
    [SerializeField] float maxVelocity;
    [SerializeField] float timePrediction;


    public bool IsOnView;

    public SteeringMode mode;
    private ISteering currentSteering;

    Vector3 steeringVelocity;

    public Vector3 SteeringVelocity { get => steeringVelocity; set => steeringVelocity = value; }


    Rigidbody rb;
    ObstacleAvoid obstacleAvoid;
    LineOfSight lineOfSight;
    EnemyPatrol enemyPatrol;

    //ExampleTeacher obstacleAv;

    public enum SteeringMode
    {
        seek,
        flee,
        persuit,
        evade
    }

    private void Awake()
    {
        obstacleAvoid = GetComponent<ObstacleAvoid>();
        rb = GetComponent<Rigidbody>();
        lineOfSight = GetComponent<LineOfSight>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    void Start()
    {
        //obstacleAv = GetComponent<ExampleTeacher>();
        Flee flee = new(rb, targetRb.transform, maxVelocity);
        Seek seek = new(rb, targetRb.transform, maxVelocity);
        Persuit persuit = new(rb, targetRb, maxVelocity, timePrediction);
        Evade evade = new(rb, targetRb, maxVelocity, timePrediction);
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
        IsOnView = targetRb && lineOfSight.CheckDistance(Target) && lineOfSight.CheckAngle(Target) && lineOfSight.CheckView(Target);

        if (IsOnView)
        {
            enemyPatrol.IsPause = true;
            enemyPatrol.IsPatrolPause = false; 
            if (obstacleAvoid.IsObstacle == false)
            {
                steeringVelocity = currentSteering.MoveDirection();
                //steeringVelocity += obstacleAv.Avoid() * steeringVelocity.magnitude;
                rb.AddForce(steeringVelocity, ForceMode.Acceleration);

                if (rb.velocity != Vector3.zero)
                {
                    transform.forward = rb.velocity;
                }
            }
            else if (obstacleAvoid.IsObstacle == true)
            {
                rb.velocity = steeringVelocity.normalized * maxVelocity;

                if (steeringVelocity != Vector3.zero)
                    transform.forward = steeringVelocity.normalized;
            }
        }
        else
        {
            enemyPatrol.IsPause = false;
            enemyPatrol.IsPatrolPause = false; 
            enemyPatrol.Patrol();
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
