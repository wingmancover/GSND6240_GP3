using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        StartScreen,
        Playing,
        LevelComplete,
        GameOver
    }

    [Header("UI References")]
    public GameObject startScreenUI;
    public GameObject endScreenUI;
    public GameObject gameOverScreenUI;

    public GameState CurrentState { get; private set; }

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
        SetState(GameState.LevelComplete);
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

    private void SetState(GameState newState)
    {
        CurrentState = newState;

        if (startScreenUI != null)
        {
            startScreenUI.SetActive(CurrentState == GameState.StartScreen);
        }

        if (endScreenUI != null)
        {
            endScreenUI.SetActive(CurrentState == GameState.LevelComplete);
        }

        if (gameOverScreenUI != null)
        {
            gameOverScreenUI.SetActive(CurrentState == GameState.GameOver);
        }
    }

    public bool IsPlaying()
    {
        return CurrentState == GameState.Playing;
    }
}