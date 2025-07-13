using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class LeaderStateFormation : LeaderState
{
    private Boid _boid;
    private AStarManager _starManager;
    private List<Node> currentPath;
    private int pathIndex = 0;
    private float reachedNodeThreshold = 0.5f;

    public LeaderStateFormation(FSMLeaderStates fsm, LeaderFSM leader, Boid boid, AStarManager aStar)
    {
        _fsm = fsm;
        _leader = leader;
        _boid = boid;
        _starManager = aStar;
    }

    public override void Awake()
    {
        _leader.SetFormationActive(true);
        _boid.enabled = true;
        _boid.isLeader = true;
        pathIndex = 0;

        if (_leader.EnemyLeaderTarget != null)
        {
            Node startNode = _starManager.GetClosestNode(_leader.transform.position);
            Node endNode = _starManager.GetClosestNode(_leader.EnemyLeaderTarget.position);

            if (startNode != null && endNode != null)
            {
                currentPath = _starManager.GeneratePath(startNode, endNode);
            }

            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Debug.DrawLine(currentPath[i].transform.position, currentPath[i + 1].transform.position, Color.red);
            }
        }

        Debug.Log("Leader: Formation state - moving towards enemy leader");
    }

    public override void Execute()
    {
        if (_leader.CanSeeEnemy())
        {
            _fsm.Transition(LeaderStateType.Attack);
            currentPath.Clear();
            return;
        }

        if (_leader.ShouldEvade())
        {
            _fsm.Transition(LeaderStateType.Evade);
            currentPath.Clear();
            return;
        }

        if (currentPath == null || currentPath.Count == 0) return;

        // Movimiento hacia el siguiente nodo del path
        Node targetNode = currentPath[pathIndex];
        Vector3 direction = (targetNode.transform.position - _leader.transform.position).normalized;
        _boid.AddForce(direction * 10f); // Ajustá la fuerza según necesidad

        float distance = Vector3.Distance(_leader.transform.position, targetNode.transform.position);
        if (distance <= reachedNodeThreshold)
        {
            pathIndex++;
            if (pathIndex >= currentPath.Count)
            {
                pathIndex = currentPath.Count - 1; // No te pasés
            }
        }

        _boid.Move();
    }
}