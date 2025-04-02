using System.Collections;
using UnityEngine;

public class ObstacleAvoid : MonoBehaviour
{
    [SerializeField] LayerMask obstacle;

    Collider[] colliders;

    void Start()
    {
        
    }

    void Update()
    {
        colliders = Physics.OverlapSphere(transform.position, 3, obstacle);
        ObstacleAvoids();
    }

    private void ObstacleAvoids()
    {
        foreach (Collider collider in colliders)
        {
            
        }
    }
}
