using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private FSMExample fsm;
    PlayerModelExample model;
    PlayerView view;

    //private PlayerStates currentState;

    private void Start()
    {
        model = GetComponent<PlayerModelExample>();
        view = GetComponent<PlayerView>();
        //currentState = PlayerStates.IdleState;
        SetFSM();
    }

    private void SetFSM()
    {
        fsm = new FSMExample();

        PlayerStateIdleExample idle = new PlayerStateIdleExample(fsm, model);
        PlayerStateMoveExample move = new PlayerStateMoveExample(fsm, model);
        PlayerStateSpinExample spin = new PlayerStateSpinExample(fsm, model);

        idle.AddTransition(PlayerStatesExample.MoveState, move);
        idle.AddTransition(PlayerStatesExample.SpinState, spin);

        move.AddTransition(PlayerStatesExample.IdleState, idle);
        move.AddTransition(PlayerStatesExample.SpinState, spin);

        spin.AddTransition(PlayerStatesExample.IdleState, idle);

        

        fsm.SetInit(idle);
    }

    private void Update()
    {
        fsm.Update();
    }
}

public enum PlayerStatesExample
{
    IdleState,
    MoveState,
    SpinState
}
