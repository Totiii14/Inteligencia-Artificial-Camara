using System.Collections;
using TMPro;
using UnityEngine;

public class RandomEvent : MonoBehaviour
{
    [SerializeField] private float invisibilityDuration = 4f;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private PlayerDetection playerDetection;

    [SerializeField] private TMP_Text textInivisibility;
    [SerializeField] private TMP_Text textDetected;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float randomValue = Random.value;
            Debug.Log($"Random Value: {randomValue}");

            if (randomValue < 0.5f)
            {
                StartCoroutine(NotifyEnemies(other.transform.position));
            }
            else
            {
                StartCoroutine(BecomeInvisibleTemporarily());
            }
        }
        GetComponent<Collider>().enabled = false;
    }

    private IEnumerator NotifyEnemies(Vector3 playerPosition)
    {
        enemyManager?.NotifyFriends(playerPosition);
        textDetected.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
        textDetected.gameObject.SetActive(false);
    }

    private IEnumerator BecomeInvisibleTemporarily()
    {
        if (playerDetection != null)
            playerDetection.SetDetectable(false);

        textInivisibility.gameObject.SetActive(true); 

        yield return new WaitForSeconds(invisibilityDuration);

        Destroy(gameObject);
        playerDetection.SetDetectable(true);
        textInivisibility.gameObject.SetActive(false);
    }
}
