using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        StartScreen,
        Playing,
        LevelCompleteSequence,
        GameOver
    }

    [Header("UI References")]
    public GameObject startScreenUI;
    public GameObject gameOverScreenUI;
    public GameObject hitUI;

    [Header("Hit UI")]
    public HitUIController hitUIController;

    [Header("Level Complete Sequence")]
    public GameObject levelCompleteSequenceRoot;
    public Image fadeBlackImage;
    public Image endBackgroundImage;
    public Image endCompleteImage;

    [Header("Level Complete Timing")]
    public float fadeDuration = 1f;
    public float backgroundFadeDuration = 0.75f;
    public float completeFadeDuration = 0.5f;

    public GameState CurrentState { get; private set; }

    private bool canClickToQuit = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SetState(GameState.StartScreen);

        if (hitUIController != null)
        {
            hitUIController.ResetHearts();
        }

        ResetLevelCompleteUI();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayStartUISound();
        }
    }

    private void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (CurrentState == GameState.StartScreen)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartGame();
            }
        }
        else if (CurrentState == GameState.GameOver)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                RestartLevel();
            }
        }
        else if (CurrentState == GameState.LevelCompleteSequence && canClickToQuit)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                QuitGame();
            }
        }
    }

    public void StartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopStartUISound();
            AudioManager.Instance.PlayBGM();
        }

        SetState(GameState.Playing);
    }

    public void CompleteLevel()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }

        SetState(GameState.LevelCompleteSequence);
        StartCoroutine(PlayLevelCompleteSequence());
    }

    public void TriggerGameOver()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }

        SetState(GameState.GameOver);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void UpdateHitUI(int hitCount)
    {
        if (hitUIController != null)
        {
            hitUIController.UpdateHearts(hitCount);
        }
    }

    public bool IsPlaying()
    {
        return CurrentState == GameState.Playing;
    }

    private void SetState(GameState newState)
    {
        CurrentState = newState;

        if (startScreenUI != null)
        {
            startScreenUI.SetActive(CurrentState == GameState.StartScreen);
        }

        if (gameOverScreenUI != null)
        {
            gameOverScreenUI.SetActive(CurrentState == GameState.GameOver);
        }

        if (hitUI != null)
        {
            hitUI.SetActive(CurrentState == GameState.Playing);
        }

        if (levelCompleteSequenceRoot != null)
        {
            levelCompleteSequenceRoot.SetActive(CurrentState == GameState.LevelCompleteSequence);
        }
    }

    private void ResetLevelCompleteUI()
    {
        canClickToQuit = false;

        if (levelCompleteSequenceRoot != null)
        {
            levelCompleteSequenceRoot.SetActive(false);
        }

        SetImageAlpha(fadeBlackImage, 0f);
        SetImageAlpha(endBackgroundImage, 0f);
        SetImageAlpha(endCompleteImage, 0f);
    }

    private IEnumerator PlayLevelCompleteSequence()
    {
        canClickToQuit = false;

        if (levelCompleteSequenceRoot != null)
        {
            levelCompleteSequenceRoot.SetActive(true);
        }

        yield return FadeImage(fadeBlackImage, 0f, 1f, fadeDuration);
        yield return FadeImage(endBackgroundImage, 0f, 1f, backgroundFadeDuration);
        yield return FadeImage(endCompleteImage, 0f, 1f, completeFadeDuration);

        canClickToQuit = true;
    }

    private IEnumerator FadeImage(Image image, float fromAlpha, float toAlpha, float duration)
    {
        if (image == null)
        {
            yield break;
        }

        float timer = 0f;
        Color color = image.color;
        color.a = fromAlpha;
        image.color = color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            color.a = Mathf.Lerp(fromAlpha, toAlpha, t);
            image.color = color;

            yield return null;
        }

        color.a = toAlpha;
        image.color = color;
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        if (image == null)
        {
            return;
        }

        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}