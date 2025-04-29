using UnityEngine;
using static PlayerStatesEnum;

public class PlayerStateAir : State
{
    private PlayerModel _model;

    public PlayerStateAir(FSM fsm, PlayerModel model)
    {
        _fsm = fsm;
        _model = model;
    }

    public override void Execute()
    {
        Vector2 input = _model.GetInput();
        Vector3 dir = new Vector3(input.x, 0f, input.y);

        if (_model.IsGrounded())
        {
            if (input.magnitude > 0.1f)
                _fsm.Transition(PlayerStates.Move);
            else
                _fsm.Transition(PlayerStates.Idle);
        }

        _model.Move(dir, _model.Data.walkSpeed * _model.Data.airMultiplier);
    }
}
