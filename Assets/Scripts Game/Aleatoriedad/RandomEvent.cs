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

    private static float weightNotify = 0.5f;
    private static float weightInvis = 0.5f;
    private float adjustAmount = 0.1f;

    private void Start()
    {
        weightNotify = 0.5f;
        weightInvis = 0.5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float totalWeight = weightNotify + weightInvis;
            float randomValue = Random.value * totalWeight;

            if (randomValue < weightNotify)
            {
                StartCoroutine(NotifyEnemies(other.transform.position));
                AdjustWeights(isInvisibility: false);
            }
            else
            {
                StartCoroutine(BecomeInvisibleTemporarily());
                AdjustWeights(isInvisibility: true);
            }
            Debug.Log($"[Weights] Notify: {weightNotify:F2} | Invisibility: {weightInvis:F2}");

            GetComponent<Collider>().enabled = false;
        }
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

    private void AdjustWeights(bool isInvisibility)
    {
        if (isInvisibility)
        {
            weightInvis = Mathf.Max(0.1f, weightInvis - adjustAmount);
            weightNotify = Mathf.Min(0.9f, weightNotify + adjustAmount);
        }
        else
        {
            weightNotify = Mathf.Max(0.1f, weightNotify - adjustAmount);
            weightInvis = Mathf.Min(0.9f, weightInvis + adjustAmount);
        }
    }
}
