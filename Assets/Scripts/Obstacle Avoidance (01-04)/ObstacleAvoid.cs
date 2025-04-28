using System.Collections;
using UnityEngine;

public class ObstacleAvoid : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float avoidForce = 10f;
    [SerializeField] private float detectDistance = 5f;
    [SerializeField] private float sphereRadius = 0.5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public Vector3 GetAvoidanceForce()
    {
        RaycastHit hit;
        Vector3 avoidance = Vector3.zero;

        if (Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hit, detectDistance, obstacleLayer))
        {
            Vector3 obstacleNormal = hit.normal;
            obstacleNormal.y = 0; // No queremos movimiento vertical

            // Calculamos un desvío lateral
            Vector3 lateralDirection = Vector3.Cross(obstacleNormal, Vector3.up).normalized;

            avoidance = lateralDirection * avoidForce;
        }

        return avoidance;
    }
}
