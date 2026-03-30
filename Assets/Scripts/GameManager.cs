using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        StartScreen,
        Playing,
        LevelComplete
    }

    [Header("UI References")]
    public GameObject startScreenUI;
    public GameObject endScreenUI;

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
    }

    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void CompleteLevel()
    {
        SetState(GameState.LevelComplete);
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
    }

    public bool IsPlaying()
    {
        return CurrentState == GameState.Playing;
    }
}