using System.Collections.Generic;

public class StateExample 
{
    protected FSMExample _fsm;
    protected Dictionary<PlayerStatesExample, StateExample> transitions = new Dictionary<PlayerStatesExample, StateExample>();

    public virtual void Awake() { }
    public virtual void Execute() { }
    public virtual void Sleep() { }

    public void AddTransition(PlayerStatesExample input, StateExample newState)
    {
        transitions[input] = newState;
    }

    //las 2 formas de remover las transiciones

    public void RemoveTransition(PlayerStatesExample input)
    {
        if (transitions.ContainsKey(input))
            transitions.Remove(input);
    }

    public void RemoveTransition(StateExample state)
    {
        foreach (KeyValuePair<PlayerStatesExample, StateExample> pair in transitions)
        {
            if (pair.Value == state)
            {
                transitions.Remove(pair.Key);
                break;
            }
        }
    }

    public StateExample GetTransition(PlayerStatesExample input)
    {
        if (transitions.TryGetValue(input, out StateExample nextState))
            return nextState;
        return null;
    }
}
