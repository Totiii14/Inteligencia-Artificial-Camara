using System.Collections;
using UnityEngine;

public class RandomEvent : MonoBehaviour
{
    [SerializeField] private float invisibilityDuration = 4f;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private PlayerDetection playerDetection;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float randomValue = Random.value;
            Debug.Log($"Random Value: {randomValue}");

            if (randomValue < 0.5f)
            {
                NotifyEnemies(other.transform.position);
            }
            else
            {
                StartCoroutine(BecomeInvisibleTemporarily());
            }
        }
        GetComponent<Collider>().enabled = false;
    }

    private void NotifyEnemies(Vector3 playerPosition)
    {
        enemyManager?.NotifyFriends(playerPosition);
    }

    private IEnumerator BecomeInvisibleTemporarily()
    {
        if (playerDetection != null)
            playerDetection.SetDetectable(false);

        Debug.Log("Se vino");

        yield return new WaitForSeconds(invisibilityDuration);

        Destroy(gameObject);
        Debug.Log("Se gue");
        playerDetection.SetDetectable(true);
    }
}
