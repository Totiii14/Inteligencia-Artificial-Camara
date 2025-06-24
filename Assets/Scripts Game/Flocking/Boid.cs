using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : SteeringBase
{
    [Header("Radiuses")]
    [SerializeField] float separationRadius;
    [SerializeField] float viewRadius;
    [Header("Weights")]
    [SerializeField, Range(0, 3f)] float separationWheight;
    [SerializeField, Range(0, 1f)] float cohesionWheight;
    [SerializeField, Range(0, 1f)] float alignmentWheight;
    [SerializeField] LayerMask boids;
    
    void Start()
    {
        Vector3 dir = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        AddForce(dir.normalized * _maxSpeed);
    }

    void Update()
    {
        Flocking();
        Move();
    }

    private void Flocking()
    {
        Vector3 flockingForce = Separation() * separationWheight + 
            Cohesion() * cohesionWheight + 
            Alignment() * alignmentWheight;
        AddForce(flockingForce);
    } 
    private Vector3 Separation()
    {
        int count = 0;
        Vector3 dir = Vector3.zero;
        var boidsInRange = Physics.OverlapSphere(transform.position, separationRadius, boids);
        for (int i = 0; i < boidsInRange.Length; i++)
        {
            Boid boid = boidsInRange[i].GetComponent<Boid>();
            if (boid == null || boid == this) continue;
            dir += (transform.position - boid.transform.position).normalized;   
            count++;
        }
        if (count == 0) return dir;
        dir /= count;
        return CalculateSteering(dir);
    }
    private Vector3 Cohesion()
    {
        int count = 0;
        Vector3 centrePos = Vector3.zero;
        var boidsInRange = Physics.OverlapSphere(transform.position, viewRadius, boids);
        for (int i = 0; i < boidsInRange.Length; i++)
        {
            Boid boid = boidsInRange[i].GetComponent<Boid>();
            if (boid == null || boid == this) continue;
            centrePos += boid.transform.position;
            count++;
        }
        if (count == 0) return centrePos;
        centrePos /= count;
        return Seek(centrePos);
    }
    private Vector3 Alignment()
    {
        int count = 0;
        Vector3 desired = Vector3.zero;
        var boidsInRange = Physics.OverlapSphere(transform.position, viewRadius, boids);
        for (int i = 0; i < boidsInRange.Length; i++)
        {
            Boid boid = boidsInRange[i].GetComponent<Boid>();
            if (boid == null || boid == this) continue;
            desired += boid.Velocity;
            count++;
        }
        if (count == 0) return desired;
        desired /= count;
        return CalculateSteering(desired.normalized * _maxSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }

}
