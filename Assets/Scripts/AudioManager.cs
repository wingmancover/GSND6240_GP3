using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource uiSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip bgmClip;
    public AudioClip hungryStomachClip;
    public AudioClip whooshClip;
    public AudioClip jumpClip;
    public AudioClip hitObstacleClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayStartUISound()
    {
        if (uiSource == null || hungryStomachClip == null)
            return;

        uiSource.clip = hungryStomachClip;
        uiSource.loop = true;
        uiSource.Play();
    }

    public void StopStartUISound()
    {
        if (uiSource != null && uiSource.isPlaying)
        {
            uiSource.Stop();
        }
    }

    public void PlayBGM()
    {
        if (bgmSource == null || bgmClip == null)
            return;

        if (bgmSource.clip != bgmClip)
        {
            bgmSource.clip = bgmClip;
        }

        bgmSource.loop = true;

        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    public void PlayWhoosh()
    {
        PlayOneShotOnSFX(whooshClip);
    }

    public void PlayJump()
    {
        PlayOneShotOnSFX(jumpClip);
    }

    public void PlayHitObstacle()
    {
        PlayOneShotOnSFX(hitObstacleClip);
    }

    private void PlayOneShotOnSFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }
}