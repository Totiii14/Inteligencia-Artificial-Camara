using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStatesEnum;

public class FSM 
{
    private State _currentState;
    //si queremos que pase algo durante la transición
    //Action<PlayerStates, State, State> onTransition = delegate { };

    public FSM() { }

    public void SetInit(State init)
    {
        _currentState = init;
        _currentState.Awake();
    }

    public void Update()
    {
        _currentState.Execute();
    }

    public void Transition(PlayerStates input)
    {
        State newState = _currentState.GetTransition(input);
        if (newState == null) return;
        _currentState.Sleep();
        // onTransition(input, _currentState, newState);
        _currentState = newState;
        _currentState.Awake();
    }
}
