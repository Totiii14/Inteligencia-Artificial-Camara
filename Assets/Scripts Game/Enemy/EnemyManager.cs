using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SteeringEntity;

public class EnemyManager : MonoBehaviour
{
    SteeringEntity steering;

    public List<SteeringEntity> persuitEnemies = new List<SteeringEntity>();
    public Vector3? lastKnownPlayerPosition = null;

    private void Awake()
    {
        steering = GetComponent<SteeringEntity>();
    }

    public void EnemyAlarm()
    {
        if (steering.mode == SteeringMode.evade)
        {
            NotifyFriends(steering.Target.position);
        }
    }

    public void NotifyFriends(Vector3 playerLastPosition)
    {
        foreach (SteeringEntity friend in persuitEnemies)
        {
            if (friend.mode == SteeringMode.persuit)
            {
                friend.enemyPatrol.IsPause = true;
                friend.GoToLastSeenPosition(playerLastPosition);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(steering.mode == SteeringMode.persuit && collision.transform.CompareTag("Player"))
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("Juego end");
        Time.timeScale = 0f;
    }
}
