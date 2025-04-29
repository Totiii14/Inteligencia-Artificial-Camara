using UnityEngine;
using static PlayerStatesEnum;

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
        _model.ApplyDrag();
    }

    public override void Execute()
    {
        Vector2 input = _model.GetInput();

        if (Input.GetKeyDown(KeyCode.Space) && _model.IsGrounded())
        {
            _model.Jump();
            _fsm.Transition(PlayerStates.Air);
        }

            if (Input.GetKey(KeyCode.LeftShift) && input.magnitude > 0.1f)
            _fsm.Transition(PlayerStates.Sprint);

        else if (input.magnitude > 0.1f)
            _fsm.Transition(PlayerStates.Move);

        if (!_model.IsGrounded())
            _fsm.Transition(PlayerStates.Air);

        _model.Move(Vector3.zero, 0f);
    }
}
