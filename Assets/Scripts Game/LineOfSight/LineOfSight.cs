using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [field: SerializeField] public Transform SecurityCamera {get; private set;}
    [field: SerializeField] public float DetectionRange {get; private set;}
    [field: SerializeField] public float DetectionAngle {get; private set;}
    [field: SerializeField] public LayerMask ObstaclesMask {get; private set;}
    [field: SerializeField] public float VerticalAngleOffset { get; private set; } = 0f;

    private void Awake()
    {
        if (SecurityCamera == null)
            SecurityCamera = transform;
    }

    public bool CheckDistance(Transform target)
    {
        if (target.GetComponent<PlayerDetection>()?.IsDetectable == false)
            return false;

        float distance = Vector3.Distance(target.position, SecurityCamera.position);
        return distance <= DetectionRange;
    }

    public bool CheckAngle(Transform target)
    {
        if (target.GetComponent<PlayerDetection>()?.IsDetectable == false)
            return false;

        Vector3 direction = target.position - SecurityCamera.position;
        Vector3 adjustedForward = Quaternion.Euler(VerticalAngleOffset, 0f, 0f) * SecurityCamera.forward;

        float angle = Vector3.Angle(adjustedForward, direction);
        return angle <= DetectionAngle / 2f;
    }

    public bool CheckView(Transform target)
    {
        if (target.GetComponent<PlayerDetection>()?.IsDetectable == false)
            return false;

        Vector3 direction = target.position - SecurityCamera.position;
        return !Physics.Raycast(SecurityCamera.position, direction.normalized, direction.magnitude, ObstaclesMask);
    }

    private void OnDrawGizmos()
    {
        if (SecurityCamera == null) return;

        Vector3 adjustedForward = Quaternion.Euler(VerticalAngleOffset, 0f, 0f) * SecurityCamera.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(SecurityCamera.position, DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(SecurityCamera.position, Quaternion.Euler(0, DetectionAngle / 2f, 0) * adjustedForward * DetectionRange);
        Gizmos.DrawRay(SecurityCamera.position, Quaternion.Euler(0, -DetectionAngle / 2f, 0) * adjustedForward * DetectionRange);
    }
}
