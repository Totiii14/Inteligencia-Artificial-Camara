using UnityEngine;
using System.Collections.Generic;
using static LeaderStatesEnum;
using static SoldierStatesEnum;

public class LeaderFSM : MonoBehaviour
{
    [Header("Leader Config")]
    [SerializeField] float detectionRadius = 15f;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] float regroupDuration = 5f;

    [SerializeField] private Transform enemyLeaderTarget;
    public Transform EnemyLeaderTarget => enemyLeaderTarget;

    [SerializeField] AStarManager aStarManager;
    [SerializeField] float timePrediction;
    [SerializeField] float maxVelocity;

    public List<IAFSM> myTroop = new();
    private FSMLeaderStates _fsm;
    private Boid _boid;
    private LineOfSight _los;
    private Rigidbody _rigidbody;
    private SteeringEntity _steeringEntity;
    private ObstacleAvoid _obstacelAvoid;

    public Transform Target;

    public int Lives => _fsm.lives;

    private void Awake()
    {
        _boid = GetComponent<Boid>();
        _los = GetComponent<LineOfSight>();
        _rigidbody = GetComponent<Rigidbody>();
        _steeringEntity = GetComponent<SteeringEntity>();
        _obstacelAvoid = GetComponent<ObstacleAvoid>();

        _fsm = new FSMLeaderStates();

        LeaderStateFormation formation = new(_fsm, this, _boid, aStarManager);
        LeaderStateAttack attack = new(_fsm, this, _rigidbody, _los, maxVelocity, timePrediction, _steeringEntity);
        LeaderStateEvade evade = new(_fsm, this, _rigidbody, _steeringEntity, _obstacelAvoid, _los, maxVelocity, timePrediction);
        LeaderStateHeal heal = new(_fsm, this);
        LeaderStatePause pause = new(_fsm, this);

        formation.AddTransition(LeaderStateType.Attack, attack);
        formation.AddTransition(LeaderStateType.Heal, heal);
        formation.AddTransition(LeaderStateType.Evade, evade);
        formation.AddTransition(LeaderStateType.Pause, pause);

        attack.AddTransition(LeaderStateType.Formation, formation);
        attack.AddTransition(LeaderStateType.Evade, evade);

        evade.AddTransition(LeaderStateType.Formation, formation);
        evade.AddTransition(LeaderStateType.Heal, heal);
        
        heal.AddTransition(LeaderStateType.Formation, formation);
        heal.AddTransition(LeaderStateType.Pause, pause);
        
        pause.AddTransition(LeaderStateType.Formation, formation);

        aStarManager.CreateConnections(FindObjectsOfType<Node>());

        _fsm.SetInit(formation);
    }

    private void Start()
    {
    }

    private void Update()
    {
        _fsm.Update();
    }

    public bool CanSeeEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyMask);
        foreach (Collider col in colliders)
        {
            if (_los.CheckDistance(col.transform) && _los.CheckAngle(col.transform) && _los.CheckView(col.transform))
            {
                Target = col.transform;
                return true;
            }
        }
        return false;
    }

    public bool ShouldEvade()
    {
        int lowLifeCount = 0;
        foreach (IAFSM soldier in myTroop)
        {
            if (soldier == null) continue;
            if (soldier.fsm.lives <= 3)
                lowLifeCount++;
        }
        return lowLifeCount >= myTroop.Count / 2f;
    }

    public void SetFormationActive(bool active)
    {
        foreach (IAFSM soldier in myTroop)
        {
            if (soldier == null) continue;
            Boid boid = soldier.GetComponent<Boid>();
            if (boid) boid.enabled = active;
        }
    }

    public void HealTroop()
    {
        foreach (IAFSM soldier in myTroop)
        {
            if (soldier == null) continue;
            FSMIASoldiers fsm = soldier.fsm;
            fsm.lives = Mathf.Min(fsm.lives + 1, 10);
        }
    }

    public void ReceiveHit()
    {
        if (_fsm.IsEvading) return;

        _fsm.lives--;
        if (_fsm.lives <= 0)
        {
            NotifyFollowers();

            Destroy(gameObject);
        }
    }

    private void NotifyFollowers()
    {
        LeaderFSM[] allLeaders = FindObjectsOfType<LeaderFSM>();
        foreach (LeaderFSM leader in allLeaders)
        {
            if (leader == this) continue;

            if (leader.GetTarget() == transform)
            {
                leader.ClearTarget();
            }
        }
    }

    public Transform GetTarget()
    {
        return _fsm.Target;
    }

    public void ClearTarget()
    {
        _fsm.Target = null;
        _fsm.Transition(LeaderStateType.Formation);
    }

    public Transform GetClosestVisibleEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyMask);
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            if (_los.CheckDistance(col.transform) && _los.CheckAngle(col.transform) && _los.CheckView(col.transform))
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = col.transform;
                }
            }
        }

        return closest;
    }
}
