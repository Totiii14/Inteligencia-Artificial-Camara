using System.Transactions;
using UnityEngine;
using static SoldierStatesEnum;

public class FSMIASoldiers 
{
    private State _currentState;

    public Transform Target { get; set; }

    public FSMIASoldiers() { }

    public void SetInit(State init)
    {
        _currentState = init;
        _currentState.Awake();
    }

    public void Update()
    {
        _currentState.Execute();
    }

    public void Transition(SoldiersIAStates input)
    {
        State newState = _currentState.GetTransition(input);
        if (newState == null) return;
        _currentState.Sleep();
        _currentState = newState;
        _currentState.Awake();
    }
}
