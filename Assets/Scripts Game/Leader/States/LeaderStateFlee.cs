using UnityEngine;

public class LeaderStateEvade : LeaderState
{
    private float timer;

    public LeaderStateEvade(FSMLeaderStates fsm, LeaderFSM leader)
    {
        _fsm = fsm;
        _leader = leader;
    }

    public override void Awake()
    {
        Debug.Log("Leader: Evading...");
        timer = 3f;
    }

    public override void Execute()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            _fsm.Transition(LeaderStateType.Heal);
        }
    }
}

