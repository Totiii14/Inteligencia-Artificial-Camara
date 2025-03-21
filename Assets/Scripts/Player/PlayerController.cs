using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private FSM fsm;
    PlayerModel model;
    PlayerView view;

    private void Start()
    {
        model = GetComponent<PlayerModel>();
        view = GetComponent<PlayerView>();
        SetFSM();
    }

    private void SetFSM()
    {
        fsm = new FSM();

        PlayerStateIdle idle = new PlayerStateIdle(fsm, model);
        PlayerStateMove move = new PlayerStateMove();
        PlayerStateSpin spin = new PlayerStateSpin();

        idle.AddTransition(PlayerStates.MoveState, move);
        idle.AddTransition(PlayerStates.SpinState, spin);

        fsm.SetInit(idle);
    }

    private void Update()
    {
        fsm.Update();
        //if (model.IsDetectable)
        //{
        //    var h = Input.GetAxis("Horizontal");
        //    var v = Input.GetAxis("Vertical");

        //    Vector3 dir = new Vector3(h, 0, v);
        //    model.Move(dir.normalized);
        //    if (h != 0 || v != 0) model.Look(dir);
        //}
        //else
        //{
        //    model.Move(Vector3.zero);
        //}
        //if (Input.GetKeyDown(KeyCode.Space)) model.Spin();
    }
}

public enum PlayerStates
{
    IdleState,
    MoveState,
    SpinState
}
