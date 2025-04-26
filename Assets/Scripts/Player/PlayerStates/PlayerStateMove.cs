using UnityEngine;

public class PlayerStateMove : State
{
    private PlayerModel _model;

    public PlayerStateMove(FSM fsm, PlayerModel model)
    {
        this._fsm = fsm;
        this._model = model;
    }

    public override void Execute()
    {
        base.Execute();

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        _model.Move(dir.normalized);

        if (h != 0 || v != 0) _model.Look(dir);
        else
            _fsm.Transition(PlayerStates.IdleState);

        if (Input.GetKeyDown(KeyCode.Space))
            _fsm.Transition(PlayerStates.SpinState);
    }
}
