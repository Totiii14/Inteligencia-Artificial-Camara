using UnityEngine;
using System.Collections.Generic;
using static LeaderStatesEnum;

public class LeaderFSM : MonoBehaviour
{
    [Header("Leader Config")]
    [SerializeField] float detectionRadius = 15f;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] float regroupDuration = 5f;

    [SerializeField] private Transform enemyLeaderTarget;
    public Transform EnemyLeaderTarget => enemyLeaderTarget;

    [SerializeField] AStarManager aStarManager;

    public List<IAFSM> myTroop = new();
    private FSMLeaderStates _fsm;
    private Boid _boid;
    private LineOfSight _los;

    private void Awake()
    {
        _boid = GetComponent<Boid>();
        _los = GetComponent<LineOfSight>();

        _fsm = new FSMLeaderStates();

        LeaderStateFormation formation = new(_fsm, this, _boid, aStarManager);
        LeaderStateAttack attack = new(_fsm, this);
        LeaderStateEvade evade = new(_fsm, this);
        LeaderStateHeal heal = new(_fsm, this);
        LeaderStatePause pause = new(_fsm, this);

        formation.AddTransition(LeaderStateType.Attack, attack);
        formation.AddTransition(LeaderStateType.Heal, heal);
        formation.AddTransition(LeaderStateType.Evade, evade);
        formation.AddTransition(LeaderStateType.Pause, pause);

        attack.AddTransition(LeaderStateType.Formation, formation);
        evade.AddTransition(LeaderStateType.Formation, formation);
        heal.AddTransition(LeaderStateType.Formation, formation);
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
}
