using System.Collections;
using UnityEngine;

public class ObstacleAvoid : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float rayDistance = 3f;
    [SerializeField] private float avoidForce = 10f;
    [SerializeField] private float frontRayOffset = 0.5f;

    public bool IsObstacle { get; private set; }

    private Vector3 avoidanceDirection = Vector3.zero;
    private float avoidCooldown = 0f;

    private void Update()
    {

        avoidCooldown -= Time.deltaTime;
        if (avoidCooldown <= 0f)
        {
            CheckForObstacles();
            avoidCooldown = 0.1f; 
        }
    }

    private void CheckForObstacles()
    {
        avoidanceDirection = Vector3.zero;
        IsObstacle = false;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3[] rayDirections = new Vector3[]
        {
            forward,                           // centro
            (forward + right).normalized,      // diagonal derecha
            (forward - right).normalized,      // diagonal izquierda
            right,                             // lateral derecha
            -right                             // lateral izquierda
        };

        foreach (Vector3 dir in rayDirections)
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f; // altura media
            if (Physics.Raycast(origin, dir, out RaycastHit hit, rayDistance, obstacleMask))
            {
                IsObstacle = true;
                avoidanceDirection += -dir * (1f - hit.distance / rayDistance);
            }
        }

        if (IsObstacle && avoidanceDirection != Vector3.zero)
        {
            avoidanceDirection = Vector3.Lerp(transform.forward, avoidanceDirection.normalized, 0.5f);
        }

        avoidanceDirection = avoidanceDirection.normalized;
    }

    public Vector3 GetAvoidDirection()
    {
        return IsObstacle ? avoidanceDirection * avoidForce : Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3[] rayDirections = new Vector3[]
        {
            forward,
            (forward + right).normalized,
            (forward - right).normalized,
            right,
            -right
        };

        foreach (Vector3 dir in rayDirections)
        {
            Gizmos.DrawRay(origin, dir * rayDistance);
        }

        if (IsObstacle)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, avoidanceDirection * avoidForce);
        }
    }
}
