using System;

public class FSMExample
{
    private StateExample _currentState;
    //si queremos que pase algo durante la transición
    //Action<PlayerStates, State, State> onTransition = delegate { };

    public FSMExample() { }

    public void SetInit(StateExample init)
    {
        _currentState = init;
        _currentState.Awake();
    }

    public void Update()
    {
        _currentState.Execute();
    }
    
    public void Transition(PlayerStatesExample input)
    {
        StateExample newState = _currentState.GetTransition(input);
        if (newState == null) return;
        _currentState.Sleep();
        // onTransition(input, _currentState, newState);
        _currentState = newState;
        _currentState.Awake();
    }
}
