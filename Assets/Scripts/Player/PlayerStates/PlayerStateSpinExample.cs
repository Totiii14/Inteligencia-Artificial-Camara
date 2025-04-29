using UnityEngine;

public class PlayerStateSpinExample : StateExample
{
    private PlayerModelExample _model;

    public PlayerStateSpinExample(FSMExample fsm, PlayerModelExample model)
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
            _fsm.Transition(PlayerStatesExample.IdleState);
    }

    public override void Sleep()
    {
        base.Sleep();
        _model.Spin(false);
    }
}
