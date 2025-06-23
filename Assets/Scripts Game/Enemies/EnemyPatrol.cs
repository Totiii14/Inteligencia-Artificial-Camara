using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] Node currentNode;
    [SerializeField] Node startNode;
    [SerializeField] Node endNode;
    [SerializeField] List<Node> path = new List<Node>();

    [SerializeField] private float maxVelocity;
    [field: SerializeField] public bool IsPause { get; set; }
    [field: SerializeField] public bool IsPatrolPause { get; set; }
    [SerializeField] bool isBack;
    
    private ObstacleAvoid obstacleAvoid;
    private Rigidbody rbEnemy;
    private bool isReturning = false;

    private void Awake()
    {
        rbEnemy = GetComponent<Rigidbody>();
        obstacleAvoid = GetComponent<ObstacleAvoid>();
        currentNode = startNode;
    }

    public void Patrol()
    {
        if (!IsPause)
        {
            if (currentNode != endNode && !isBack)
            {
                CreatePath();
            }
            else if (currentNode == endNode && !isBack && !isReturning)
            {
                StartCoroutine(PauseAtEnd(() => BackToStart()));
            }
            else if (currentNode != startNode && isBack)
            {
                CreatePath();
            }
            else if (currentNode == startNode && isBack && !isReturning)
            {
                StartCoroutine(PauseAtEnd(() => isBack = false));
            }

            if (obstacleAvoid.IsObstacle)
            {
                Vector3 avoidDir = obstacleAvoid.GetAvoidDirection().normalized;
                rbEnemy.velocity = avoidDir * maxVelocity;

                if (avoidDir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(avoidDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }

                return; 
            }
        }
        else if (IsPatrolPause)
        {
            rbEnemy.velocity = Vector3.zero;
        }
    }

    public void CreatePath()
    {
        if (path.Count > 0)
        {
            int x = 0;
            Vector3 toTarget = path[x].transform.position - transform.position;
            toTarget.y = 0;
            Vector3 direction = toTarget.normalized;

            rbEnemy.velocity = direction * maxVelocity;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, path[x].transform.position) < 0.05f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        }
        else
        {
            Node[] nodes = FindObjectsOfType<Node>();

            if (!AStarManager.instance.IsAllNodes)
            {
                AStarManager.instance.CreateConnections(nodes);
                AStarManager.instance.IsAllNodes = true;
            }

            List<Node> intermediateNodes = new List<Node>();

            Node randomNode = nodes[UnityEngine.Random.Range(0, nodes.Length)];
            if (randomNode != currentNode && randomNode != endNode && !intermediateNodes.Contains(randomNode))
            {
                intermediateNodes.Add(randomNode);
            }

            List<Node> totalPath = new List<Node>();

            Node lastNode = currentNode;

            foreach (Node intermediate in intermediateNodes)
            {
                List<Node> partialPath = AStarManager.instance.GeneratePath(lastNode, intermediate);
                if (partialPath != null && partialPath.Count > 0)
                {
                    totalPath.AddRange(partialPath);
                    lastNode = intermediate;
                }
            }

            List<Node> finalPath = AStarManager.instance.GeneratePath(lastNode, endNode);
            if (finalPath != null && finalPath.Count > 0)
            {
                totalPath.AddRange(finalPath);
            }

            path = totalPath;
        }
    }

    public void BackToStart()
    {
        isReturning = true;
        isBack = true;

        List<Node> backPath = AStarManager.instance.GeneratePath(currentNode, startNode);
        if (backPath != null && backPath.Count > 0)
        {
            path.AddRange(backPath);
        }
        isReturning = false;
    }

    private IEnumerator PauseAtEnd(System.Action afterPauseAction)
    {
        IsPause = true;
        rbEnemy.velocity = Vector3.zero;

        yield return new WaitForSeconds(3f);

        IsPause = false;
        afterPauseAction?.Invoke(); 
    }
}

