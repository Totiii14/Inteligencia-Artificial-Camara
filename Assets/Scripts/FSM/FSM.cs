using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FSM
{
    private State _currentState;
    Action<PlayerStates, State, State> onTransition = delegate { };

    public FSM() { }
    public FSM(State init)
    {
        SetInit(init);
    }
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
        onTransition(input, _currentState, newState);
        _currentState = newState;
        _currentState.Awake();
    }
}
