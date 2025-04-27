using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Rotación")]
    [SerializeField] private float rotationSpeed = 45f; // grados por segundo

    [Header("Detección")]
    [SerializeField] private Transform securityCamera; // <- La referencia al objeto que mira
    private LineOfSight lineOfSight;
    [SerializeField] private Transform player;
    [SerializeField] private EnemyManager enemyManager;

    private void Awake()
    {
        lineOfSight = GetComponentInChildren<LineOfSight>();
    }

    private void Update()
    {
        RotateVision();
        DetectPlayer();
    }

    private void RotateVision()
    {
        if (securityCamera == null) return;

        securityCamera.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void DetectPlayer()
    {
        if (player == null || lineOfSight == null || enemyManager == null) return;

        bool isInDistance = lineOfSight.CheckDistance(player);
        bool isInAngle = lineOfSight.CheckAngle(player);
        bool isInView = lineOfSight.CheckView(player);

        if (isInDistance && isInAngle && isInView)
        {
            Debug.Log("¡Jugador detectado!");
            enemyManager.NotifyFriends(player.position);
        }
    }
}
