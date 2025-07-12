using UnityEngine;
using static SoldierStatesEnum;

public class SoldierStateSearching : State
{
    private FSMIASoldiers _fsm;
    private Rigidbody _rb;
    private Boid _boid;

    private LineOfSight lineOfSight;
    private float detectionRadius = 10f;
    private string enemyTag;

    private Transform currentTarget;

    public SoldierStateSearching(FSMIASoldiers fsm, Boid boid, Rigidbody rb, LineOfSight los, string enemyTag)
    {
        _fsm = fsm;
        _boid = boid;
        _rb = rb;
        this.lineOfSight = los;
        this.enemyTag = enemyTag;
    }

    public override void Awake()
    {
        _boid.enabled = true;
    }

    public override void Execute()
    {
        Collider[] colliders = Physics.OverlapSphere(_rb.position, detectionRadius);

        foreach (Collider col in colliders)
        {
            if (!col.CompareTag(enemyTag)) continue;

            Transform target = col.transform;

            if (lineOfSight.CheckDistance(target) &&
                lineOfSight.CheckAngle(target) &&
                lineOfSight.CheckView(target))
            {
                _boid.enabled = false;
                currentTarget = target;
                _fsm.Target = target;
                _fsm.Transition(SoldiersIAStates.Chasing);
                return;
            }
        }
    }

    public override void Sleep()
    {
        _boid.enabled = false;
    }
}
