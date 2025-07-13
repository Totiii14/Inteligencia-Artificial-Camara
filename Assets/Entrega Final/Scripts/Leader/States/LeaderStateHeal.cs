using UnityEngine;

public class LeaderStateHeal : LeaderState
{
    private float timer;

    public LeaderStateHeal(FSMLeaderStates fsm, LeaderFSM leader)
    {
        _fsm = fsm;
        _leader = leader;
    }

    public override void Awake()
    {
        Debug.Log("Leader: Healing...");
        timer = 1f;
        int healsToPerform = 3;
        for (int i = 0; i < healsToPerform; i++)
        {
            _leader.HealTroop();
        }
    }

    public override void Execute()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            _fsm.Transition(LeaderStateType.Pause);
        }
    }
}

