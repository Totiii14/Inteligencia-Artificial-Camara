using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static SoldierStatesEnum;

public class SoldierStateSearching : State
{
    private FSMIASoldiers _fsm;
    private Rigidbody _rb;
    private Boid _boid;
    private ObstacleAvoid _obstacleAvoid;
    private LineOfSight lineOfSight;
    private float detectionRadius = 10f;
    private string enemyTag;
    private float searchTimer = 0f;
    private float searchTimeout = 8f;

    private List<Node> pathToEnemy = new();
    private int pathIndex = 0;

    private AStarManager _aStar;
    private Animator _animator;

    private Transform currentTarget;

    private bool shouldRegroup = false;
    private Transform regroupTarget = null;

    public SoldierStateSearching(FSMIASoldiers fsm, Boid boid, AStarManager aStar, Rigidbody rb, LineOfSight los, ObstacleAvoid obstacleAvoid, Animator animator, string enemyTag)
    {
        _fsm = fsm;
        _boid = boid;
        _rb = rb;
        _aStar = aStar;
        _obstacleAvoid = obstacleAvoid;
        _animator = animator;
        this.lineOfSight = los;
        this.enemyTag = enemyTag;
    }

    public override void Awake()
    {
        _boid.enabled = true;
        searchTimer = 0f;
        pathIndex = 0;
        pathToEnemy.Clear();

        _animator.SetTrigger("Searching");
    }

    public override void Execute()
    {
        searchTimer += Time.deltaTime;

        if (shouldRegroup && regroupTarget != null)
        {
            Vector3 dir = (regroupTarget.position - _rb.position).normalized;
            Vector3 avoidForce = _obstacleAvoid.GetAvoidDirection();
            Vector3 finalDir = dir * 10f + avoidForce;

            _boid.AddForce(finalDir);
            _boid.Move();

            if (Vector3.Distance(_rb.position, regroupTarget.position) <= 2f)
            {
                shouldRegroup = false; 
                regroupTarget = null;
            }

            return; 
        }

        Collider[] colliders = Physics.OverlapSphere(_rb.position, detectionRadius);

        foreach (Collider col in colliders)
        {
            if (!col.CompareTag(enemyTag)) continue;

            Transform target = col.transform;

            if (lineOfSight.CheckDistance(target) &&
                lineOfSight.CheckAngle(target) &&
                lineOfSight.CheckView(target))
            {
                _boid.enabled = false;
                currentTarget = target;
                _fsm.Target = target;
                _fsm.Transition(SoldiersIAStates.Chasing);
                return;
            }
        }

        if (_fsm.lives <= 3 && !_fsm.hasEscaped)
        {
            _fsm.hasEscaped = true;
            _fsm.Transition(SoldiersIAStates.Evading);
            return;
        }

        if (searchTimer >= searchTimeout && pathToEnemy.Count == 0)
        {
            Transform randomEnemy = GetRandomEnemy();

            if (randomEnemy != null)
            {
                Node start = _aStar.GetClosestNode(_rb.position);
                Node end = _aStar.GetClosestNode(randomEnemy.position);
                if (start != null && end != null)
                {
                    pathToEnemy = _aStar.GeneratePath(start, end);
                    pathIndex = 0;
                    searchTimer = 0f;
                }
            }
        }

        if (pathToEnemy != null && pathToEnemy.Count > 0 && pathIndex < pathToEnemy.Count)
        {
            Node targetNode = pathToEnemy[pathIndex];
            Vector3 dir = (targetNode.transform.position - _rb.position).normalized;
            Vector3 avoidForce = _obstacleAvoid.GetAvoidDirection();
            Vector3 finalDir = dir * 10f + avoidForce;

            _boid.AddForce(finalDir);
            _boid.Move();

            float dist = Vector3.Distance(_rb.position, targetNode.transform.position);
            if (dist <= 0.5f)
            {
                pathIndex++;
            }

            if (pathIndex >= pathToEnemy.Count)
            {
                pathToEnemy.Clear();
                pathIndex = 0;
                searchTimer = searchTimeout; 
            }
        }
    }

    private Transform GetRandomEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        if (enemies.Length == 0) return null;
        return enemies[Random.Range(0, enemies.Length)].transform;
    }

    public void SetRegroupTarget(Transform leader)
    {
        regroupTarget = leader;
        shouldRegroup = true;
    }

    public override void Sleep()
    {
        _boid.enabled = false;
    }
}
