using UnityEngine;

public class LeaderStatePause : LeaderState
{
    private float pauseTimer;

    public LeaderStatePause(FSMLeaderStates fsm, LeaderFSM leader)
    {
        _fsm = fsm;
        _leader = leader;
    }

    public override void Awake()
    {
        pauseTimer = 2f;
        Debug.Log("Leader: Strategic pause");
    }

    public override void Execute()
    {
        pauseTimer -= Time.deltaTime;
        if (pauseTimer <= 0)
        {
            _fsm.Transition(LeaderStateType.Formation);
        }
    }
}
