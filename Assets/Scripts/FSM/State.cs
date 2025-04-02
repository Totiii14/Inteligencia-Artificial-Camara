using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State 
{
    protected FSM _fsm;
    protected Dictionary<PlayerStates, State> transitions = new Dictionary<PlayerStates, State>();

    public virtual void Awake() { }
    public virtual void Execute() { }
    public virtual void Sleep() { }

    public void AddTransition(PlayerStates input, State newState)
    {
        transitions[input] = newState;
    }

    public void RemoveTransition(PlayerStates input)
    {
        if (transitions.ContainsKey(input))
            transitions.Remove(input);
    }

    public void RemoveTransition(State state)
    {
        foreach (var pair in transitions)
        {
            if (pair.Value == state)
            {
                transitions.Remove(pair.Key);
                break;
            }
        }
    }

    public State GetTransition(PlayerStates input)
    {
        if (transitions.TryGetValue(input, out State nextState))
            return nextState;
        return null;
    }
}
