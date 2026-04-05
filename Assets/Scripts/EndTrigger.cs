using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsPlaying())
        {
            Debug.Log("Level Complete!");
            GameManager.Instance.CompleteLevel();
        }
    }
}