using UnityEngine;
using static SoldierStatesEnum;
using static SteeringEntity;

public class SoldierStateChasing : State
{
    private SteeringEntity _steering;
    private ObstacleAvoid _obstacleAvoid;
    private Rigidbody _rb;
    private Transform _transform;
    private LineOfSight _los;
    private float _maxVelocity;
    private float _timePrediction;

    public SoldierStateChasing(FSMIASoldiers fsm, SteeringEntity steering, Rigidbody rb, ObstacleAvoid obstacleAvoid, Transform transform, LineOfSight los, float maxVelocity, float timePrediction)
    {
        _fsm = fsm;
        _steering = steering;
        _obstacleAvoid = obstacleAvoid;
        _rb = rb;
        _transform = transform;
        _los = los;
        _maxVelocity = maxVelocity;
        _timePrediction = timePrediction;
    }

    public override void Awake()
    {
        _steering.mode = SteeringMode.persuit;
    }

    public override void Execute()
    {
        if (_fsm.Target == null) return;

        if (!_los.CheckDistance(_fsm.Target) ||
            !_los.CheckAngle(_fsm.Target) ||
            !_los.CheckView(_fsm.Target))
        {
            _fsm.Transition(SoldiersIAStates.SearchingEnemy);
            return;
        }

        if (_fsm.lives <= 3 && !_fsm.hasEscaped)
        {
            _fsm.hasEscaped = true;
            _fsm.Transition(SoldiersIAStates.Evading);
            return;
        }

        if (!_obstacleAvoid.IsObstacle)
        {
            Persuit persuit = new(_rb, _fsm.Target.GetComponent<Rigidbody>(), _maxVelocity, _timePrediction);
            _steering.currentSteering = persuit;

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

        if ((_rb.position - _fsm.Target.position).sqrMagnitude < 2f)
        {
            _rb.velocity = Vector3.zero;

            float hitChance = 0.7f;
            if (Random.value < hitChance)
            {
                IAFSM enemyIA = _fsm.Target.GetComponent<IAFSM>();
                if (enemyIA != null)
                {
                    enemyIA.ReceiveHit();
                }
            }
            else
            {
                Debug.Log($"{_rb.name} falló el ataque.");
            }
        }
        else
        {
            _rb.AddForce(_steering.SteeringVelocity, ForceMode.Acceleration);
        }

        if (_steering.SteeringVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_steering.SteeringVelocity.normalized);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}

