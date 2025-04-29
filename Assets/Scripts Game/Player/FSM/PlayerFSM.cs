using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStatesEnum;

public class PlayerFSM : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float crouchSpeed = 2f;
    public float jumpForce = 5f;
    public float groundDrag = 5f;
    public float airMultiplier = 0.5f;
    public float playerHeight = 2f;
    public LayerMask groundLayer;

    [Header("Orientation")]
    public Transform orientation;

    private Rigidbody rb;
    private FSM fsm;
    private PlayerModel model;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        model = new PlayerModel(this, rb);
    }

    private void Start()
    {
        rb.freezeRotation = true;

        fsm = new FSM();

        PlayerStateIdle idle = new PlayerStateIdle(fsm, model);
        PlayerStateWalk walk = new PlayerStateWalk(fsm, model);
        PlayerStateSprint sprint = new PlayerStateSprint(fsm, model);
        PlayerStateAir air = new PlayerStateAir(fsm, model);

        // --- TRANSICIONES ---
        idle.AddTransition(PlayerStates.Move, walk);
        idle.AddTransition(PlayerStates.Sprint, sprint);
        idle.AddTransition(PlayerStates.Air, air);

        walk.AddTransition(PlayerStates.Idle, idle);
        walk.AddTransition(PlayerStates.Sprint, sprint);
        walk.AddTransition(PlayerStates.Air, air);

        sprint.AddTransition(PlayerStates.Idle, idle);
        sprint.AddTransition(PlayerStates.Move, walk);
        sprint.AddTransition(PlayerStates.Air, air);

        air.AddTransition(PlayerStates.Idle, idle);
        air.AddTransition(PlayerStates.Move, walk);
        air.AddTransition(PlayerStates.Sprint, sprint);

        fsm.SetInit(idle);
    }

    private void Update()
    {
        fsm.Update();
    }
}
