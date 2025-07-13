using UnityEngine;
using static SoldierStatesEnum;
using static SteeringEntity;

public class SoldierStateEvading : State
{
    private SteeringEntity _steering;
    private ObstacleAvoid _obstacleAvoid;
    private Rigidbody _rb;
    private Transform _transform;
    private float _maxVelocity;
    private float _timePrediction;

    private float evadeDuration = 3f;
    private float evadeTimer = 0f;

    public SoldierStateEvading(FSMIASoldiers fsm, SteeringEntity steering, Rigidbody rb, ObstacleAvoid obstacleAvoid, Transform transform, float maxVelocity, float timePrediction)
    {
        _fsm = fsm;
        _steering = steering;
        _obstacleAvoid = obstacleAvoid;
        _rb = rb;
        _transform = transform;
        _maxVelocity = maxVelocity;
        _timePrediction = timePrediction;
    }

    public override void Awake()
    {

        Debug.Log("Evade");
        _steering.mode = SteeringMode.evade;
        _maxVelocity = 10f;
        evadeTimer = 0f;
        _fsm.SetEvading(true); 
    }

    public override void Execute()
    {
        if (_fsm.Target == null) return;

        if (!_obstacleAvoid.IsObstacle)
        {
            Evade evade = new(_rb, _fsm.Target.GetComponent<Rigidbody>(), _maxVelocity, _timePrediction);
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

        evadeTimer += Time.deltaTime;
        if (evadeTimer >= evadeDuration)
        {
            _fsm.SetEvading(false);
            _fsm.Transition(SoldiersIAStates.SearchingEnemy);
            return;
        }
    }
}
