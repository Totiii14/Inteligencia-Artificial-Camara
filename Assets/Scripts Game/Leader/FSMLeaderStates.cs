
public class FSMLeaderStates 
{
    private LeaderState _currentState;

    public void SetInit(LeaderState state)
    {
        _currentState = state;
        _currentState.Awake();
    }

    public void Update()
    {
        _currentState.Execute();
    }

    public void Transition(LeaderStateType input)
    {
        LeaderState newState = _currentState.GetTransition(input);
        if (newState == null) return;
        _currentState.Sleep();
        _currentState = newState;
        _currentState.Awake();
    }
}

public enum LeaderStateType
{
    Formation,
    Attack,
    Evade,
    Heal,
    Pause
}
