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


    private Rigidbody rbEnemy;
    [SerializeField] private int currentPatrolIndex = 0;
    [field: SerializeField] public bool IsPause { get; set; }
    [field: SerializeField] public bool IsPatrolPause { get; set; } 
    [SerializeField] private bool isPatrollingForward = true;

    private void Awake()
    {
        rbEnemy = enemy.GetComponent<Rigidbody>();
    }

    public void Patrol()
    {
        if (patrolPath == null || patrolPath.Count == 0) return;

        if (!IsPause)
        {
            Transform currentTarget = patrolPath[currentPatrolIndex].transform;

            Vector3 direction = (currentTarget.position - enemy.transform.position).normalized * maxVelocity;
            direction.y = 0;

            rbEnemy.velocity = direction;

            if (direction != Vector3.zero)
            {
                enemy.transform.forward = direction;
            }

            float distanceToTarget = Vector3.Distance(enemy.transform.position, currentTarget.position);
            if (distanceToTarget < distancePoint)
            {
                StartCoroutine(PauseBeforeNextPoint());
            }
        }
        else
        {
            if (IsPatrolPause)
            {
                rbEnemy.velocity = Vector3.zero;
            }
        }
    }

    private IEnumerator PauseBeforeNextPoint()
    {
        if (currentPatrolIndex == patrolPath.Count - 1 && isPatrollingForward)
        {
            IsPause = true;
            IsPatrolPause = true;
            rbEnemy.velocity = Vector3.zero; // <- me aseguro de frenarlo justo al llegar
            yield return new WaitForSeconds(pauseTime);
            IsPause = false;
            isPatrollingForward = false;
        }
        else if (currentPatrolIndex == 0 && !isPatrollingForward)
        {
            IsPause = true;
            IsPatrolPause = true;
            rbEnemy.velocity = Vector3.zero; // <- me aseguro de frenarlo justo al llegar
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

