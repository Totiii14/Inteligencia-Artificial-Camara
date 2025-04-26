using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] List<GameObject> patrolPath;
    [SerializeField] private int maxVelocity;
    [SerializeField] private float distancePoint = 1f;
    [SerializeField] private float pauseTime = 2f;


    private Rigidbody rb;
    private int currentPatrolIndex = 0;
    public bool IsPause { get; set; }
    private bool isPatrollingForward = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Patrol()
    {
        if (patrolPath == null || patrolPath.Count == 0) return;

        if (!IsPause)
        {
            Transform currentTarget = patrolPath[currentPatrolIndex].transform;

            Vector3 direction = (currentTarget.position - transform.position).normalized * maxVelocity;
            direction.y = 0;

            rb.velocity = direction;

            if (direction != Vector3.zero)
            {
                transform.forward = direction;
            }

            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget < distancePoint)
            {
                StartCoroutine(PauseBeforeNextPoint());
            }
        }
    }

    private IEnumerator PauseBeforeNextPoint()
    {
        if (currentPatrolIndex == patrolPath.Count - 1 && isPatrollingForward)
        {
            IsPause = true;
            yield return new WaitForSeconds(pauseTime);
            IsPause = false;
            isPatrollingForward = false;
        }
        else if (currentPatrolIndex == 0 && !isPatrollingForward)
        {
            IsPause = true;
            yield return new WaitForSeconds(pauseTime);
            IsPause = false;
            isPatrollingForward = true;
        }

        if (isPatrollingForward)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPath.Count;
        }
        else if (!isPatrollingForward)
        {
            currentPatrolIndex = (currentPatrolIndex - 1 + patrolPath.Count) % patrolPath.Count;
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

