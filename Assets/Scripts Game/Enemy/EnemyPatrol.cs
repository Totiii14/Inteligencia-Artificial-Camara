using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] List<GameObject> patrolPath;
    [SerializeField] GameObject enemy;
    [SerializeField] private int maxVelocity;
    [SerializeField] private float distancePoint = 1f;
    [SerializeField] private float pauseTime = 2f;
    private SteeringEntity steering;
    private ObstacleAvoid obstacleAvoid;


    private Rigidbody rbEnemy;
    [SerializeField] private int currentPatrolIndex = 0;
    [field: SerializeField] public bool IsPause { get; set; }
    [field: SerializeField] public bool IsPatrolPause { get; set; } 
    [SerializeField] private bool isPatrollingForward = true;

    private void Awake()
    {
        rbEnemy = enemy.GetComponent<Rigidbody>();
        steering = enemy.GetComponent<SteeringEntity>();
        obstacleAvoid = enemy.GetComponent<ObstacleAvoid>();
    }

    public void Patrol()
    {
        if (patrolPath == null || patrolPath.Count == 0) return;

        if (!IsPause)
        {
            Transform currentTarget = patrolPath[currentPatrolIndex].transform;
            Vector3 toTarget = (currentTarget.position - enemy.transform.position);
            toTarget.y = 0;

            // Dirección deseada normalizada
            Vector3 desiredDirection = toTarget.normalized;

            // Obtener fuerza de evitación (ya normalizada)
            Vector3 avoidance = obstacleAvoid.GetAvoidanceForce();

            // Combinar direcciones ponderando la evitación
            Vector3 finalDirection = Vector3.zero;

            if (avoidance != Vector3.zero)
            {
                // Priorizar evitación cuando hay obstáculos cercanos
                finalDirection = avoidance.normalized;
            }
            else
            {
                // Usar dirección normal al patrullar
                finalDirection = desiredDirection;
            }

            // Aplicar velocidad manteniendo la magnitud constante
            rbEnemy.velocity = finalDirection * maxVelocity;

            // Rotación suave hacia la dirección final
            if (finalDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(finalDirection);
                enemy.transform.rotation = Quaternion.Slerp(
                    enemy.transform.rotation,
                    targetRotation,
                    Time.deltaTime * 5f
                );
            }

            float distanceToTarget = toTarget.magnitude;
            if (distanceToTarget < distancePoint)
            {
                StartCoroutine(PauseBeforeNextPoint());
            }
        }
        else if (IsPatrolPause)
        {
            rbEnemy.velocity = Vector3.zero;
        }
    }

    private IEnumerator PauseBeforeNextPoint()
    {
        if (currentPatrolIndex == patrolPath.Count - 1 && isPatrollingForward)
        {
            IsPause = true;
            IsPatrolPause = true;
            rbEnemy.velocity = Vector3.zero; 
            yield return new WaitForSeconds(pauseTime);
            IsPause = false;
            isPatrollingForward = false;
        }
        else if (currentPatrolIndex == 0 && !isPatrollingForward)
        {
            IsPause = true;
            IsPatrolPause = true;
            rbEnemy.velocity = Vector3.zero; 
            yield return new WaitForSeconds(pauseTime);
            IsPause = false;
            isPatrollingForward = true;
        }
        else
        {
            if (isPatrollingForward)
            {
                currentPatrolIndex++;
            }
            else
            {
                currentPatrolIndex--;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (patrolPath == null || patrolPath.Count == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < patrolPath.Count; i++)
        {
            if (patrolPath[i] != null)
                Gizmos.DrawSphere(patrolPath[i].transform.position, 0.3f);

            if (i < patrolPath.Count - 1)
                Gizmos.DrawLine(patrolPath[i].transform.position, patrolPath[i + 1].transform.position);
        }
    }
}

