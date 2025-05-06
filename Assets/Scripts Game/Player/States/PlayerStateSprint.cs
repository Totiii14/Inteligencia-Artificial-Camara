using UnityEngine;
using static PlayerStatesEnum;

public class PlayerStateSprint : State
{
    private PlayerModel _model;

    public PlayerStateSprint(FSM fsm, PlayerModel model)
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
        Vector3 dir = new Vector3(input.x, 0f, input.y);

        if (!Input.GetKey(KeyCode.LeftShift) || input.magnitude < 0.1f)
            _fsm.Transition(PlayerStates.Move);

        _model.Move(dir, _model.Data.sprintSpeed);
    }
}
