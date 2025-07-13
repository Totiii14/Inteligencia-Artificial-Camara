using System.Collections;
using UnityEngine;

public class SteeringEntity : MonoBehaviour
{
    [field: SerializeField] public Transform Target { get; private set; }
    [SerializeField] private Rigidbody targetRb;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float timePrediction;

    public SteeringMode mode;
    public ISteering currentSteering;
    private Vector3 steeringVelocity;

    public Rigidbody rb { get; set; }
    private ObstacleAvoid obstacleAvoid;
    private LineOfSight lineOfSight;
    private EnemyManager enemyManager;

    private bool IsChasing = false;
    public bool IsOnView { get; private set; }
    public Vector3 SteeringVelocity { get => steeringVelocity; set => steeringVelocity = value; }

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
        enemyManager = GetComponent<EnemyManager>();
    }

    void Start()
    {
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

        if (IsOnView)
        {
            IsChasing = true;
            enemyManager.EnemyAlarm();

            if (!obstacleAvoid.IsObstacle)
            {
                steeringVelocity = currentSteering.MoveDirection();
            }
            else
            {
                Vector3 avoidDir = obstacleAvoid.GetAvoidDirection();
                if (avoidDir != Vector3.zero)
                    steeringVelocity = avoidDir;
                else
                    steeringVelocity = Vector3.zero;
            }

            rb.AddForce(steeringVelocity, ForceMode.Acceleration);

            if (steeringVelocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(steeringVelocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
        else
        {
            IsChasing = false;
        }
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
}
