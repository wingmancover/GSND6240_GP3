using UnityEngine;

public class ProximityAudioTrigger : MonoBehaviour
{
    public Transform player;
    public AudioSource audioSource;
    public float triggerDistanceZ = 10f;
    public bool triggerOnlyOnce = true;

    private bool hasPlayed = false;

    private void Update()
    {
        if (hasPlayed && triggerOnlyOnce)
        {
            return;
        }

        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
        {
            return;
        }

        if (player == null || audioSource == null)
        {
            return;
        }

        float zDifference = transform.position.z - player.position.z;

        if (zDifference <= triggerDistanceZ)
        {
            audioSource.Play();
            hasPlayed = true;
        }
    }
}