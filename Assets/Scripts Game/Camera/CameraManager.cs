using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Rotación")]
    [SerializeField] private float rotationSpeed = 45f; 
    [SerializeField] private float minYRotation = -26f;
    [SerializeField] private float maxYRotation = 40f;
    private bool rotatingRight = true;

    [Header("Detección")]
    private LineOfSight lineOfSight;
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private bool isOnView;
    [SerializeField] private EnemyManager enemyManager;

    private IDesitionNode rootNode;

    private void Awake()
    {
        lineOfSight = GetComponentInChildren<LineOfSight>();
        BuildBehaviorTree();
    }

    private void Update()
    {
        rootNode.Execute();
    }

    private void BuildBehaviorTree()
    {
        ActionNode patrolPan = new ActionNode(PatrolPan);
        ActionNode playerDetected = new ActionNode(OnPlayerDetected);

        QuestionNode detectPlayer = new QuestionNode(
            playerDetected,
            patrolPan,
            DetectPlayer
        );

        rootNode = detectPlayer;
    }

    private bool DetectPlayer()
    {
        if (playerRb == null || lineOfSight == null) return false;

        isOnView = playerRb && lineOfSight.CheckDistance(player) && lineOfSight.CheckAngle(player) && lineOfSight.CheckView(player);

        return isOnView;
    }

    private void OnPlayerDetected()
    {
        Debug.Log("¡Jugador detectado!");
        enemyManager?.NotifyFriends(player.position);
    }

    private void PatrolPan()
    {
        Vector3 currentRotation = transform.localEulerAngles;
        float yRotation = NormalizeAngle(currentRotation.y);

        if (rotatingRight)
        {
            yRotation += rotationSpeed * Time.deltaTime;
            if (yRotation >= maxYRotation)
            {
                yRotation = maxYRotation;
                rotatingRight = false;
            }
        }
        else
        {
            yRotation -= rotationSpeed * Time.deltaTime;
            if (yRotation <= minYRotation)
            {
                yRotation = minYRotation;
                rotatingRight = true;
            }
        }

        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }
        return angle;
    }
}
