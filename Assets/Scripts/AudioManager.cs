using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource uiSource;
    public AudioSource uiSecondarySource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip bgmClip;
    public AudioClip hungryStomachClip;
    public AudioClip startUIExtraClip;
    public AudioClip whooshClip;
    public AudioClip jumpClip;
    public AudioClip hitObstacleClip;
    public AudioClip levelCompleteClip;

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

    public void PlayStartUISounds()
    {
        if (uiSource != null && hungryStomachClip != null)
        {
            uiSource.clip = hungryStomachClip;
            uiSource.loop = false;
            uiSource.Play();
        }

        if (uiSecondarySource != null && startUIExtraClip != null)
        {
            uiSecondarySource.clip = startUIExtraClip;
            uiSecondarySource.loop = false;
            uiSecondarySource.Play();
        }
    }

    public void StopStartUISounds()
    {
        if (uiSource != null && uiSource.isPlaying)
        {
            uiSource.Stop();
        }

        if (uiSecondarySource != null && uiSecondarySource.isPlaying)
        {
            uiSecondarySource.Stop();
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

    public void PlayLevelComplete()
    {
        if (uiSecondarySource == null || levelCompleteClip == null)
            return;

        uiSecondarySource.PlayOneShot(levelCompleteClip);
    }

    private void PlayOneShotOnSFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }
}