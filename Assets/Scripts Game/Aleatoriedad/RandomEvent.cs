using System.Collections;
using UnityEngine;

public class RandomEvent : MonoBehaviour
{
    [SerializeField] private float invisibilityDuration = 4f;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private PlayerDetection playerDetection;
    private bool hasActivated = false;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            hasActivated = true;

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

            Destroy(gameObject);
        }
    }

    private void NotifyEnemies(Vector3 playerPosition)
    {
        enemyManager?.NotifyFriends(playerPosition);
    }

    private IEnumerator BecomeInvisibleTemporarily()
    {
        if (playerDetection != null)
            playerDetection.SetDetectable(false);

        yield return new WaitForSeconds(invisibilityDuration);

        playerDetection.SetDetectable(true);
    }
}
