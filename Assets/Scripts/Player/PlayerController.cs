using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private FSM fsm;
    PlayerModel model;
    PlayerView view;

    //private PlayerStates currentState;

    private void Start()
    {
        model = GetComponent<PlayerModel>();
        view = GetComponent<PlayerView>();
        //currentState = PlayerStates.IdleState;
        SetFSM();
    }

    private void SetFSM()
    {
        fsm = new FSM();

        PlayerStateIdle idle = new PlayerStateIdle(fsm, model);
        PlayerStateMove move = new PlayerStateMove(fsm, model);
        PlayerStateSpin spin = new PlayerStateSpin(fsm, model);

        idle.AddTransition(PlayerStates.MoveState, move);
        idle.AddTransition(PlayerStates.SpinState, spin);

        move.AddTransition(PlayerStates.IdleState, idle);
        move.AddTransition(PlayerStates.SpinState, spin);

        spin.AddTransition(PlayerStates.IdleState, idle);

        

        fsm.SetInit(idle);
    }

    private void Update()
    {
        fsm.Update();
    }
}

public enum PlayerStates
{
    IdleState,
    MoveState,
    SpinState
}
