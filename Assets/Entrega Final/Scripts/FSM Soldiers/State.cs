using System.Collections.Generic;
using static SoldierStatesEnum;

public class State 
{
    protected FSMIASoldiers _fsm;
    protected Dictionary<SoldiersIAStates, State> transitions = new Dictionary<SoldiersIAStates, State>();

    public virtual void Awake() { }
    public virtual void Execute() { }
    public virtual void Sleep() { }

    public void AddTransition(SoldiersIAStates input, State newState)
    {
        transitions[input] = newState;
    }

    public State GetTransition(SoldiersIAStates input)
    {
        if (transitions.TryGetValue(input, out State nextState))
            return nextState;
        return null;
    }
}
