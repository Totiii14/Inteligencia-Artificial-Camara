using UnityEngine;
using System.Collections.Generic;
using static SoldierStatesEnum;

public class LeaderFSM : MonoBehaviour
{
    [Header("Leader Config")]
    [SerializeField] float detectionRadius = 15f;
    public LayerMask enemyMask;
    [SerializeField] float regroupDuration = 5f;

    [Header("Troop Spawn")]
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private int soldiersToSpawn = 5;
    [SerializeField] private Transform spawnPoint;

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
    private Animator _animator;

    public Transform Target;

    public int Lives => _fsm.lives;

    private void Awake()
    {
        _boid = GetComponent<Boid>();
        _los = GetComponent<LineOfSight>();
        _rigidbody = GetComponent<Rigidbody>();
        _steeringEntity = GetComponent<SteeringEntity>();
        _obstacelAvoid = GetComponent<ObstacleAvoid>();
        _animator = GetComponentInChildren<Animator>();

        _fsm = new FSMLeaderStates();

        LeaderStateFormation formation = new(_fsm, this, _boid, aStarManager, _obstacelAvoid, _animator);
        LeaderStateAttack attack = new(_fsm, this, _rigidbody, _los, _obstacelAvoid, maxVelocity, timePrediction, _steeringEntity, _animator);
        LeaderStateEvade evade = new(_fsm, this, _rigidbody, _steeringEntity, _obstacelAvoid, _los, _animator, maxVelocity, timePrediction);
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
        SpawnTroop();
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
        List<IAFSM> candidates = new();
        List<float> weights = new();

        foreach (IAFSM soldier in myTroop)
        {
            if (soldier == null) continue;

            int life = soldier.fsm.lives;
            candidates.Add(soldier);

            float weight = 0f;

            if (life <= 3) 
                weight = 6f;
            else if (life <= 6) 
                weight = 3f;
            else
                weight = 1f;

            weights.Add(weight);
        }

        IAFSM selected = WeightedRandomSelection(candidates, weights);
        if (selected == null) return;

        int currentLife = selected.fsm.lives;

        int healAmount = 0;
        if (currentLife <= 3)
        {
            healAmount = Random.Range(3, 6);
        }
        else if (currentLife <= 6)
        {
            healAmount = Random.Range(1, 4);
        }
        else
        {
            healAmount = Random.Range(0, 2); 
        }

        selected.fsm.lives = Mathf.Min(10, selected.fsm.lives + healAmount);

        Debug.Log($"Leader curó a {selected.name} con {healAmount} puntos de vida (ahora tiene {selected.fsm.lives})");
    }

    private IAFSM WeightedRandomSelection(List<IAFSM> candidates, List<float> weights)
    {
        if (candidates.Count != weights.Count || candidates.Count == 0)
            return null;

        float totalWeight = 0f;
        foreach (float w in weights) totalWeight += w;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < candidates.Count; i++)
        {
            cumulative += weights[i];
            if (randomValue <= cumulative)
            {
                return candidates[i];
            }
        }

        return candidates[candidates.Count - 1]; // fallback
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

    public void CommandRegroup()
    {
        foreach (IAFSM soldier in myTroop)
        {
            if (soldier == null) continue;

            if (soldier.fsm != null && soldier.fsm.GetCurrentState() is SoldierStateSearching searchingState)
            {
                searchingState.SetRegroupTarget(transform);
            }
        }
    }

    private void SpawnTroop()
    {
        for (int i = 0; i < soldiersToSpawn; i++)
        {
            Vector3 offset = Random.insideUnitSphere * 2f;
            offset.y = 0;

            GameObject soldierGO = Instantiate(soldierPrefab, spawnPoint.position + offset, Quaternion.identity);
            IAFSM soldier = soldierGO.GetComponent<IAFSM>();

            if (soldier != null)
            {
                myTroop.Add(soldier);
            }
        }
    }
}
