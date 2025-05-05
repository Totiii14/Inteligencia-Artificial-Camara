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
        Vector3[] directions = new Vector3[]
    {
    transform.forward,
    transform.right,
    -transform.right,
    -transform.forward,
    (transform.forward + transform.right).normalized,
    (transform.forward - transform.right).normalized,
    (-transform.forward + transform.right).normalized,
    (-transform.forward - transform.right).normalized
    };

        foreach (Vector3 dir in directions)
        {
            if (!Physics.Raycast(transform.position, dir, 7f, obstacle))
            {
                lastAvoidDirection = dir;
                return lastAvoidDirection;
            }
        }

        return lastAvoidDirection != Vector3.zero ? lastAvoidDirection : -transform.forward;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);

        Vector3[] directions = new Vector3[]
        {
    transform.forward,
    transform.right,
    -transform.right,
    -transform.forward,
    (transform.forward + transform.right).normalized,
    (transform.forward - transform.right).normalized,
    (-transform.forward + transform.right).normalized,
    (-transform.forward - transform.right).normalized
        };

        Gizmos.color = Color.yellow;
        foreach (var dir in directions)
        {
            Gizmos.DrawRay(transform.position, dir * 7f);
        }
    }
}
