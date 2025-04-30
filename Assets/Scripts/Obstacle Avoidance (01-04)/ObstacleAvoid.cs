using System.Collections;
using UnityEngine;

public class ObstacleAvoid : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float detectDistance = 3f;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float rayAngle = 30f;
    [SerializeField] private int rayCount = 3; // Número de rayos por lado

    [Header("Avoidance")]
    [SerializeField] private float avoidForce = 10f;
    [SerializeField] private float smoothTime = 0.3f; // Tiempo de suavizado
    [SerializeField] private float minForceThreshold = 0.1f; // Fuerza mínima para aplicar

    private Rigidbody rb;
    private Vector3 currentAvoidance;
    private Vector3 avoidanceVelocity;

    private void Awake() => rb = GetComponent<Rigidbody>();

    public Vector3 GetAvoidanceForce()
    {
        Vector3 desiredAvoidance = CalculateAvoidance();

        // Suavizar la transición entre fuerzas
        currentAvoidance = Vector3.SmoothDamp(
            currentAvoidance,
            desiredAvoidance,
            ref avoidanceVelocity,
            smoothTime
        );

        // Solo aplicar si supera el umbral mínimo
        return currentAvoidance.magnitude > minForceThreshold ? currentAvoidance : Vector3.zero;
    }

    private Vector3 CalculateAvoidance()
    {
        Vector3 movementDirection = rb.velocity.normalized;
        if (movementDirection == Vector3.zero) movementDirection = transform.forward;

        Vector3 bestDirection = Vector3.zero;
        float bestScore = 0f;

        // Rayo central principal
        if (Physics.SphereCast(transform.position, sphereRadius, movementDirection,
            out RaycastHit mainHit, detectDistance, obstacleLayer))
        {
            float danger = 1f - (mainHit.distance / detectDistance);

            // Evaluar múltiples direcciones potenciales
            for (int i = 0; i <= rayCount; i++)
            {
                float angle = rayAngle * (i / (float)rayCount);

                // Probar ambos lados
                EvaluateDirection(Quaternion.AngleAxis(angle, Vector3.up) * movementDirection, danger, ref bestDirection, ref bestScore);
                EvaluateDirection(Quaternion.AngleAxis(-angle, Vector3.up) * movementDirection, danger, ref bestDirection, ref bestScore);
            }

            // Si no encontramos buena dirección, retroceder
            if (bestScore <= 0)
            {
                bestDirection = -movementDirection * 0.5f;
                bestScore = danger * 0.3f;
            }
        }

        return bestDirection * avoidForce * bestScore;
    }

    private void EvaluateDirection(Vector3 direction, float danger, ref Vector3 bestDirection, ref float bestScore)
    {
        if (!Physics.Raycast(transform.position, direction, detectDistance * 0.8f, obstacleLayer))
        {
            // Puntuar dirección basada en qué tan lejos está de obstáculos y alineación con movimiento
            float score = danger * (1f - Vector3.Angle(direction, transform.forward) / 180f);

            if (score > bestScore)
            {
                bestScore = score;
                bestDirection = direction;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;
        Vector3 direction = rb.velocity.normalized;
        if (direction == Vector3.zero) direction = transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + direction * detectDistance);
        Gizmos.DrawWireSphere(transform.position + direction * detectDistance, sphereRadius);

        // Dibujar direcciones evaluadas
        Gizmos.color = Color.cyan;
        for (int i = 0; i <= rayCount; i++)
        {
            float angle = rayAngle * (i / (float)rayCount);
            Vector3 dir1 = Quaternion.AngleAxis(angle, Vector3.up) * direction;
            Vector3 dir2 = Quaternion.AngleAxis(-angle, Vector3.up) * direction;

            Gizmos.DrawLine(transform.position, transform.position + dir1 * detectDistance * 0.8f);
            Gizmos.DrawLine(transform.position, transform.position + dir2 * detectDistance * 0.8f);
        }

        // Dibujar dirección de evitación actual
        if (currentAvoidance != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + currentAvoidance.normalized * 2f);
        }
    }
}
