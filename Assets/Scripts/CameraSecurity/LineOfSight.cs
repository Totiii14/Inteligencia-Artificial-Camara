using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private Transform securityCamera;
    [SerializeField] private float detectionRange;
    [SerializeField] private float detectionAngle;
    [SerializeField] private LayerMask obstaclesMask;
    [SerializeField] private float verticalAngleOffset = 0f;

    private void Awake()
    {
        if (securityCamera == null)
            securityCamera = transform;
    }

    public bool CheckDistance(Transform target)
    {
        float distance = Vector3.Distance(target.position, securityCamera.position);
        return distance <= detectionRange;
    }

    public bool CheckAngle(Transform target)
    {
        Vector3 direction = target.position - securityCamera.position;
        Vector3 adjustedForward = Quaternion.Euler(verticalAngleOffset, 0f, 0f) * securityCamera.forward;

        float angle = Vector3.Angle(adjustedForward, direction);
        return angle <= detectionAngle / 2f;
    }

    public bool CheckView(Transform target)
    {
        Vector3 direction = target.position - securityCamera.position;
        return !Physics.Raycast(securityCamera.position, direction.normalized, direction.magnitude, obstaclesMask);
    }

    private void OnDrawGizmos()
    {
        if (securityCamera == null) return;

        Vector3 adjustedForward = Quaternion.Euler(verticalAngleOffset, 0f, 0f) * securityCamera.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(securityCamera.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(securityCamera.position, Quaternion.Euler(0, detectionAngle / 2f, 0) * adjustedForward * detectionRange);
        Gizmos.DrawRay(securityCamera.position, Quaternion.Euler(0, -detectionAngle / 2f, 0) * adjustedForward * detectionRange);
    }
}
