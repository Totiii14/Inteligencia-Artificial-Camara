using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LeaderState 
{
    protected FSMLeaderStates _fsm;
    protected LeaderFSM _leader;
    private Dictionary<LeaderStateType, LeaderState> _transitions = new();

    public void AddTransition(LeaderStateType type, LeaderState state) => _transitions[type] = state;
    public LeaderState GetTransition(LeaderStateType input) => _transitions.TryGetValue(input, out var state) ? state : null;

    public abstract void Awake();
    public abstract void Execute();
    public virtual void Sleep() { }
}
