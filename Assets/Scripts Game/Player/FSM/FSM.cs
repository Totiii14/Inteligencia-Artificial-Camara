using static PlayerStatesEnum;

public class FSM 
{
    private State _currentState;

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
        _currentState = newState;
        _currentState.Awake();
    }
}
