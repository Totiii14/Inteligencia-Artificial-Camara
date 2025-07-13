using UnityEngine;

public class LeaderStateAttack : LeaderState
{
    private Transform _target;
    private Rigidbody _rb;
    private LineOfSight _los;
    private float _maxVelocity;
    private float _timePrediction;
    private SteeringEntity _steering;

    public LeaderStateAttack(FSMLeaderStates fsm, LeaderFSM leader, Rigidbody rb, LineOfSight los, float maxVelocity, float timePrediction, SteeringEntity steering)
    {
        _fsm = fsm;
        _leader = leader;
        _rb = rb;
        _los = los;
        _maxVelocity = maxVelocity;
        _timePrediction = timePrediction;
        _steering = steering;
    }

    public override void Awake()
    {
        _leader.SetFormationActive(false);
        _target = _leader.GetClosestVisibleEnemy();
        Debug.Log("Leader: Attack!");
    }

    public override void Execute()
    {

        if (_leader.Lives <= 3 && !_fsm.hasEscaped)
        {
            _fsm.hasEscaped = true;
            _fsm.Transition(LeaderStateType.Evade);
            return;
        }

        if (_target == null)
        {
            _target = _leader.GetClosestVisibleEnemy();
            if (_target == null)
            {
                _fsm.Transition(LeaderStateType.Formation);
                return;
            }
        }

        if (!_leader.CanSeeEnemy())
        {
            _fsm.Transition(LeaderStateType.Formation);
            return;
        }

        Persuit persuit = new(_rb, _target.GetComponent<Rigidbody>(), _maxVelocity, _timePrediction);
        _steering.currentSteering = persuit;
        _steering.SteeringVelocity = _steering.currentSteering.MoveDirection();

        _rb.AddForce(_steering.SteeringVelocity, ForceMode.Acceleration);

        if (_steering.SteeringVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_steering.SteeringVelocity.normalized);
            _leader.transform.rotation = Quaternion.Slerp(_leader.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Ataque si está cerca
        if ((_rb.position - _target.position).sqrMagnitude < 2f)
        {
            _rb.velocity = Vector3.zero;

            float hitChance = 0.6f;
            if (Random.value < hitChance)
            {
                if (_target.TryGetComponent<IAFSM>(out IAFSM iAFSM))
                {
                    iAFSM.ReceiveHit();
                }
                else if (_target.TryGetComponent<LeaderFSM>(out LeaderFSM leaderFSM))
                {
                    leaderFSM.ReceiveHit();
                }
            }
            else
            {
            }
        }
    }
}
