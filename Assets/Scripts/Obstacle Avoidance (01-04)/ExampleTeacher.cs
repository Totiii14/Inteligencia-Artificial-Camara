using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleTeacher : MonoBehaviour
{
    [SerializeField] float detectionRange;
    [SerializeField] float avoidForce;
    [SerializeField] LayerMask obstacleMask;


    private void Start()
    {
        Avoid();
    }

    public Vector3 Avoid()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, obstacleMask);

        float minDist = detectionRange + 1;
        Collider closestColl = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            float currentDist = Vector3.Distance(transform.position, colliders[i].transform.position);
            if (currentDist < minDist)
            {
                closestColl = colliders[i];
                minDist = currentDist;
            }
        }
        if (closestColl == null) return Vector3.zero;

        Vector3 avoidDir = (closestColl.transform.position - transform.position).normalized * avoidForce;
        avoidDir.y = 0;
        avoidDir *= Mathf.Lerp(1, 0, Vector3.Distance(transform.position, closestColl.transform.position) / detectionRange);
        return avoidDir;
    }
}
