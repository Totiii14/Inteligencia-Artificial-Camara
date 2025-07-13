using UnityEngine;
using UnityEngine.Playables;
using static SoldierStatesEnum;

public class IAFSM : MonoBehaviour
{
    [Header("Movement Settings")]
    public string enemyTag;

    [SerializeField] float maxVelocity;
    [SerializeField] float timePrediction;
    AStarManager AStarManager;

    private Rigidbody rb;
    private Boid boid;
    private SteeringEntity steering;
    public FSMIASoldiers fsm;
    private LineOfSight los;
    private ObstacleAvoid obstacleAvoid;
    private Animator _animator;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boid = GetComponent<Boid>();
        steering = GetComponent<SteeringEntity>();  
        los = GetComponent<LineOfSight>();
        obstacleAvoid = GetComponent<ObstacleAvoid>();
        _animator = GetComponentInChildren<Animator>();

        AStarManager = FindObjectOfType<AStarManager>();
    }

    private void Start()
    {
        rb.freezeRotation = true;

        fsm = new FSMIASoldiers();

        
        SoldierStateSearching searching = new SoldierStateSearching(fsm, boid, AStarManager, rb, los, obstacleAvoid, _animator, enemyTag);
        SoldierStateChasing chasing = new SoldierStateChasing(fsm, steering, rb, obstacleAvoid, transform, los, _animator, maxVelocity, timePrediction);
        SoldierStateEvading evading = new SoldierStateEvading(fsm, steering, rb, obstacleAvoid, _animator, transform, maxVelocity, timePrediction);

        searching.AddTransition(SoldiersIAStates.Chasing, chasing);
        searching.AddTransition(SoldiersIAStates.Evading, evading);

        chasing.AddTransition(SoldiersIAStates.SearchingEnemy, searching);
        chasing.AddTransition(SoldiersIAStates.Evading, evading);

        evading.AddTransition(SoldiersIAStates.SearchingEnemy, searching);
        evading.AddTransition(SoldiersIAStates.Chasing, chasing);
        

        fsm.SetInit(searching);
        
    }

    private void Update()
    {
        fsm.Update();
    }

    public void ReceiveHit()
    {
        if (fsm.IsEvading) return;

        fsm.lives--;
        if (fsm.lives <= 0)
        {
            NotifyFollowers();

            Destroy(gameObject);
        }
    }

    private void NotifyFollowers()
    {
        IAFSM[] allSoldiers = FindObjectsOfType<IAFSM>();
        foreach (IAFSM soldier in allSoldiers)
        {
            if (soldier == this) continue;

            if (soldier.GetTarget() == transform)
            {
                soldier.ClearTarget();
            }
        }
    }

    public Transform GetTarget()
    {
        return fsm.Target;
    }

    public void ClearTarget()
    {
        fsm.Target = null;
        fsm.Transition(SoldiersIAStates.SearchingEnemy);
    }
}
