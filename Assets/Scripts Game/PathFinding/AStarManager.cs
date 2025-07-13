using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;

    [field: SerializeField] public bool IsAllNodes { get; set; }

    private void Awake()
    {
        instance = this;
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();

        foreach (Node n in FindObjectsOfType<Node>())
        {
            n.gScore = float.MaxValue;
        }

        start.gScore = 0;
        start.hScore = Vector3.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            int lowestF = default;

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                {
                    lowestF = i;
                }
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if (currentNode == end)
            {
                List<Node> path = new List<Node>();
                path.Insert(0, end);

                while (currentNode != start)
                {
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                }

                path.Reverse();

                return CleanPath(path);
            }

            foreach (Node connectedNode in currentNode.connections)
            {
                float heldGScore = currentNode.gScore + Vector3.Distance(currentNode.transform.position, connectedNode.transform.position);

                if (heldGScore < connectedNode.gScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector3.Distance(connectedNode.transform.position, end.transform.position);

                    if (!openSet.Contains(connectedNode))
                    {
                        openSet.Add(connectedNode);
                    }
                }
            }
        }

        return null;
    }

    public void CreateConnections(Node[] nodeList)
    {
        for (int i = 0; i < nodeList.Length; i++)
        {
            for (int j = i + 1; j < nodeList.Length; j++)
            {
                if (Vector3.Distance(nodeList[i].transform.position, nodeList[j].transform.position) <= 5f)
                {
                    ConnectNodes(nodeList[i], nodeList[j]);
                    ConnectNodes(nodeList[j], nodeList[i]);
                }
            }
        }
    }

    private void ConnectNodes(Node from, Node to)
    {
        if (from == to) return;

        if (!from.connections.Contains(to))
            from.connections.Add(to);

    }

    public Node GetClosestNode(Vector3 position)
    {
        Node[] allNodes = FindObjectsOfType<Node>();
        Node closest = null;
        float minDist = float.MaxValue;

        foreach (Node node in allNodes)
        {
            float dist = Vector3.Distance(position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }

    public List<Node> CleanPath(List<Node> rawPath)
    {
        if (rawPath == null || rawPath.Count <= 2)
            return rawPath;

        List<Node> cleanedPath = new();
        cleanedPath.Add(rawPath[0]);

        int index = 0;

        while (index < rawPath.Count - 1)
        {
            int nextIndex = rawPath.Count - 1;

            for (int i = rawPath.Count - 1; i > index; i--)
            {
                if (HasLineOfSight(rawPath[index].transform.position, rawPath[i].transform.position))
                {
                    nextIndex = i;
                    break;
                }
            }

            cleanedPath.Add(rawPath[nextIndex]);
            index = nextIndex;
        }

        return cleanedPath;
    }

    private bool HasLineOfSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        float distance = dir.magnitude;

        return !Physics.Raycast(start, dir.normalized, distance, LayerMask.GetMask("Obstacle"));
    }
}
