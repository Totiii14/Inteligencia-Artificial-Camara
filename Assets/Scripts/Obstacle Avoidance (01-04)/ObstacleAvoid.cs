using System.Collections;
using UnityEngine;

public class ObstacleAvoid : MonoBehaviour
{
    [SerializeField] LayerMask obstacle;
    [SerializeField] float distance;

    [SerializeField] bool isObstacle;

    Collider[] colliders;

    public bool IsObstacle { get => isObstacle; set => isObstacle = value; }

    void Update()
    {
        colliders = Physics.OverlapSphere(transform.position, 3, obstacle);
        ObstacleAvoids();
    }

    private void ObstacleAvoids()
    {
        foreach (Collider collider in colliders)
        {
            ReturnDistance(collider);
        }
    }

    private void ReturnDistance(Collider collider)
    {
        Vector3 closestPoint = collider.ClosestPoint(transform.position);
        distance = Vector3.Distance(transform.position, closestPoint);

        if(distance < 1f)
        {
            isObstacle = true;
            SteeringEntity collEntity = GetComponent<SteeringEntity>();
            collEntity.DesiredVelocity = NewDirection();
        }
        else
            isObstacle = false;
    }

    private Vector3 NewDirection()
    {
        Ray fwd = new(transform.position, transform.forward);
        Ray right = new(transform.position, transform.right);
        Ray left = new(transform.position, -transform.right);
        Ray back = new(transform.position, -transform.forward);

        bool rayForward = Physics.Raycast(fwd, 7f, obstacle);
        bool rayBack = Physics.Raycast(back, 7f, obstacle);
        bool rayRight = Physics.Raycast(right, 7f, obstacle);
        bool rayLeft = Physics.Raycast(left, 7f, obstacle);

        if (!rayForward)
        {
            Debug.Log("adelante");
            return fwd.direction;
        }
        else if (!rayRight)
        {
            Debug.Log("rights");
            return right.direction;
        }
        else if (!rayLeft)
        {
            Debug.Log("left");
            return left.direction;
        }
        else if (!rayBack)
        {
            Debug.Log("back");
            return back.direction;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
