using System.Collections;
using UnityEngine;

public class ObstacleAvoid : MonoBehaviour
{
    [SerializeField] private LayerMask obstacle;
    [SerializeField] private float distance;

    [SerializeField] private bool isObstacle;

    private Collider[] colliders;

    private Vector3 lastAvoidDirection;

    public bool IsObstacle { get => isObstacle; set => isObstacle = value; }

    private void Update()
    {
        colliders = Physics.OverlapSphere(transform.position, 3, obstacle);
        ObstacleAvoids();

        if (!isObstacle)
        {
            lastAvoidDirection = Vector3.zero;
        }
    }

    private void ObstacleAvoids()
    {
        float closestDistance = float.MaxValue;
        Collider closestCollider = null;

        foreach (Collider collider in colliders)
        {
            float dist = Vector3.Distance(transform.position, collider.ClosestPoint(transform.position));
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestCollider = collider;
            }
        }

        if (closestCollider != null && closestDistance < 1f)
        {
            isObstacle = true;
            SteeringEntity collEntity = GetComponent<SteeringEntity>();
            collEntity.SteeringVelocity = NewDirection();
        }
        else
        {
            isObstacle = false;
        }
    }

    public Vector3 NewDirection()
    {
        Ray fwd = new(transform.position, transform.forward);
        Ray right = new(transform.position, transform.right);
        Ray left = new(transform.position, -transform.right);
        Ray back = new(transform.position, -transform.forward);

        bool rayForward = Physics.Raycast(fwd, 7f, obstacle);
        bool rayBack = Physics.Raycast(back, 7f, obstacle);
        bool rayRight = Physics.Raycast(right, 7f, obstacle);
        bool rayLeft = Physics.Raycast(left, 7f, obstacle);

        if (!rayForward)
            lastAvoidDirection = fwd.direction;
        else if (!rayRight)
            lastAvoidDirection = right.direction;
        else if (!rayLeft)
            lastAvoidDirection = left.direction;
        else if (!rayBack)
            lastAvoidDirection = back.direction;

        return lastAvoidDirection;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}
