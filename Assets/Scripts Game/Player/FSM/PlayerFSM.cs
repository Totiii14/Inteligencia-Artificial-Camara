using UnityEngine;
using static PlayerStatesEnum;

public class PlayerFSM : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float groundDrag = 5f;
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

        idle.AddTransition(PlayerStates.Move, walk);
        idle.AddTransition(PlayerStates.Sprint, sprint);

        walk.AddTransition(PlayerStates.Idle, idle);
        walk.AddTransition(PlayerStates.Sprint, sprint);

        sprint.AddTransition(PlayerStates.Idle, idle);
        sprint.AddTransition(PlayerStates.Move, walk);

        fsm.SetInit(idle);
    }

    private void Update()
    {
        fsm.Update();
    }
}
