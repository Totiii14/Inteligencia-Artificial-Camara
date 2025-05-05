using System.Collections.Generic;
using static PlayerStatesEnum;

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

    public State GetTransition(PlayerStates input)
    {
        if (transitions.TryGetValue(input, out State nextState))
            return nextState;
        return null;
    }
}
