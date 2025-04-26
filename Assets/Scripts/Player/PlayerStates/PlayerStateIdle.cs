using UnityEngine;

public class PlayerStateIdle : State
{
    private PlayerModel _model;

    public PlayerStateIdle(FSM fsm, PlayerModel model)
    {
        _fsm = fsm;
        _model = model;
    }
    public override void Awake()
    {
        base.Awake();
        _model.Move(Vector3.zero);
    }
    public override void Execute()
    {
        base.Execute();

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
            _fsm.Transition(PlayerStates.MoveState);

        if (Input.GetKeyDown(KeyCode.Space))
            _fsm.Transition(PlayerStates.SpinState);
    }
}
