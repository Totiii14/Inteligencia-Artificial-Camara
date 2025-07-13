using UnityEngine;
using static SteeringEntity;

public class LeaderStateEvade : LeaderState
{
    private Rigidbody _rb;
    private SteeringEntity _steering;
    private ObstacleAvoid _obstacleAvoid;
    private Transform _transform;
    private float _maxVelocity;
    private float _timePrediction;
    private LineOfSight _los;

    private float evadeDuration = 3f;
    private float evadeTimer = 0f;

    public LeaderStateEvade(FSMLeaderStates fsm, LeaderFSM leader,
        Rigidbody rb, SteeringEntity steering, ObstacleAvoid obstacleAvoid, LineOfSight los, float maxVelocity, float timePrediction)
    {
        _fsm = fsm;
        _leader = leader;
        _rb = rb;
        _steering = steering;
        _obstacleAvoid = obstacleAvoid;
        _transform = leader.transform;
        _los = los;
        _maxVelocity = maxVelocity;
        _timePrediction = timePrediction;
    }


    public override void Awake()
    {
        Debug.Log("Leader: Evading...");
        _fsm.SetEvading(true);
        evadeTimer = 0f;

        _steering.mode = SteeringMode.evade;

        if (_leader.Target == null)
        {
            _leader.Target = _leader.GetClosestVisibleEnemy();
            Debug.Log("Target set for evade: " + _leader.Target?.name);
        }
        else
        {
            Debug.Log("Target es: "+ _leader.Target.name);
        }
    }

    public override void Execute()
    {
        // Si no hay objetivo al que evadir, buscarlo
        if (_leader.Target == null)
        {
            _leader.Target = _leader.GetClosestVisibleEnemy();
            if (_leader.Target == null)
            {
                _fsm.SetEvading(false);
                _fsm.Transition(LeaderStateType.Heal);
                return;
            }
        }

        if (!_obstacleAvoid.IsObstacle)
        {
            Evade evade = new(_rb, _leader.Target.GetComponent<Rigidbody>(), _maxVelocity, _timePrediction);
            _steering.currentSteering = evade;

            _steering.SteeringVelocity = _steering.currentSteering.MoveDirection();
        }
        else
        {
            Vector3 avoidDir = _obstacleAvoid.GetAvoidDirection();
            if (avoidDir != Vector3.zero)
                _steering.SteeringVelocity = avoidDir;
            else
                _steering.SteeringVelocity = Vector3.zero;
        }

        _rb.AddForce(_steering.SteeringVelocity, ForceMode.Acceleration);

        if (_steering.SteeringVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_steering.SteeringVelocity.normalized);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Timer de duración
        evadeTimer += Time.deltaTime;
        if (evadeTimer >= evadeDuration)
        {
            _fsm.SetEvading(false);
            _fsm.Transition(LeaderStateType.Heal);
        }
    }
}


