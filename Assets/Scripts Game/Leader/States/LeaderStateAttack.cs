using UnityEngine;

public class LeaderStateAttack : LeaderState
{
    public LeaderStateAttack(FSMLeaderStates fsm, LeaderFSM leader)
    {
        _fsm = fsm;
        _leader = leader;
    }

    public override void Awake()
    {
        _leader.SetFormationActive(false);
        Debug.Log("Leader: Attack!");
    }

    public override void Execute()
    {
        if (!_leader.CanSeeEnemy())
        {
            _fsm.Transition(LeaderStateType.Formation);
        }
    }
}
