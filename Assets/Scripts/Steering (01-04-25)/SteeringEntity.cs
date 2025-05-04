using System.Collections;
using UnityEngine;

public class SteeringEntity : MonoBehaviour
{
    [field: SerializeField] public Transform Target { get; private set; }
    [SerializeField] Rigidbody targetRb;
    [SerializeField] float maxVelocity;
    [SerializeField] float timePrediction;

    public SteeringMode mode;
    public ISteering currentSteering;
    Vector3 steeringVelocity;

    public Rigidbody rb;
    ObstacleAvoid obstacleAvoid;
    LineOfSight lineOfSight;
    public EnemyPatrol enemyPatrol { get; private set; }
    EnemyManager enemyManager;

    private bool IsChasing = false;
    public bool IsOnView;
    public Vector3 SteeringVelocity { get => steeringVelocity; set => steeringVelocity = value; }

    private Coroutine backToPatrolCoroutine;

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
        enemyManager = GetComponent<EnemyManager>();
    }

    void Start()
    {
        //obstacleAv = GetComponent<ExampleTeacher>();
        Persuit persuit = new(rb, targetRb, maxVelocity, timePrediction);
        Evade evade = new(rb, targetRb, maxVelocity, timePrediction);
        switch (mode)
        {
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

        if (IsOnView || (currentSteering is Persuit p && p.IsOverridingTarget))
        {
            if (backToPatrolCoroutine != null)
            {
                StopCoroutine(backToPatrolCoroutine);
                backToPatrolCoroutine = null;
            }

            IsChasing = true;
            enemyPatrol.IsPause = true;
            enemyPatrol.IsPatrolPause = false;
            enemyManager.EnemyAlarm();

            if (currentSteering is Persuit persuitSteering && persuitSteering.IsOverridingTarget)
            {
                if (Vector3.Distance(transform.position, persuitSteering.TargetPosition) < 1.5f)
                {
                    persuitSteering.ResetTarget();
                    if (backToPatrolCoroutine == null)
                        backToPatrolCoroutine = StartCoroutine(BackToPatrol());
                }
            }

            if (!obstacleAvoid.IsObstacle)
            {
                steeringVelocity = currentSteering.MoveDirection();
            }
            else
            {
                Vector3 avoidDirection = obstacleAvoid.NewDirection(); 
                steeringVelocity = avoidDirection * maxVelocity;
            }

            rb.AddForce(steeringVelocity, ForceMode.Acceleration);

            if (steeringVelocity != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(steeringVelocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
        else
        {
            IsChasing = false;
            if (enemyPatrol.IsPause)
            {
                if (backToPatrolCoroutine == null)
                    backToPatrolCoroutine = StartCoroutine(BackToPatrol());
            }
            else
            {
                enemyPatrol.IsPause = false;
                enemyPatrol.IsPatrolPause = false;
                enemyPatrol.Patrol();
            }
        }
    }

    private IEnumerator BackToPatrol()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        yield return new WaitForSeconds(3f);

        if (!IsChasing) 
        {
            enemyPatrol.IsPause = false;
            enemyPatrol.IsPatrolPause = false;
            enemyPatrol.Patrol();
        }

        backToPatrolCoroutine = null; 
    }

    public void GoToLastSeenPosition(Vector3 lastPosition)
    {
        if (mode == SteeringMode.persuit)
        {
            if (currentSteering is Persuit persuitSteering)
            {
                persuitSteering.OverrideTarget(lastPosition);
            }
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
