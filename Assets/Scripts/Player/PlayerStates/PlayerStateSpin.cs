using UnityEngine;

public class PlayerStateSpin : State
{
    private PlayerModel _model;

    public PlayerStateSpin(FSM fsm, PlayerModel model)
    {
        this._fsm = fsm;
        this._model = model;
    }

    public override void Awake()
    {
        base.Awake();
        _model.Spin(true);
    }

    public override void Execute()
    {
        base.Execute();

        if (Input.GetKeyDown(KeyCode.Space))
            _fsm.Transition(PlayerStates.IdleState);
    }

    public override void Sleep()
    {
        base.Sleep();
        _model.Spin(false);
    }
}
