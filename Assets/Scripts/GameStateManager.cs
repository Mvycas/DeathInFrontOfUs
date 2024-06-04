using System;
using MovementSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public enum GameState
    {
        GameStart,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    private GameState _currentState;
    private CameraController cameraController;
    private bool _gameStarted;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; 
            _gameStarted = false; 

            //CurrentState = GameState.GameStart;
        }
    }
    
    void OnEnable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;  // kinda to avoid duplicates... But not sure if I rlly need this rn.... since I have onDisable cleanup....
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;  
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (cameraController == null) {
            cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        }

        // Check if this is truly the first load of the game
        if (!_gameStarted) {
            _gameStarted = true;  
            CurrentState = GameState.GameStart;
        } else {
            CurrentState = GameState.Playing;
        }
    }

    public GameState CurrentState
    {
        get { return _currentState; }
        private set
        {
            _currentState = value;
            HandleStateChange(_currentState);
        }
    }

    private void HandleStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.GameStart:
                Debug.Log("GameStateManager: sends UI MANAGER TO UPDATE UI WITH game start state");
                Time.timeScale = 0; 
                UIManager.Instance.UpdateUI(state);
                break;
            case GameState.Playing:
                Time.timeScale = 1; 
                UIManager.Instance.UpdateUI(state);
                break;
            case GameState.Paused:
                Time.timeScale = 0; 
                UIManager.Instance.UpdateUI(state);
                break;
            case GameState.GameOver:
                Time.timeScale = 0; 
                UIManager.Instance.UpdateUI(state);
                break;
            case GameState.Victory:
                Time.timeScale = 0; 
                UIManager.Instance.UpdateUI(state);
                break;
        }
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            cameraController.SetPause(true);
            CurrentState = GameState.Paused;
        }
        else if (CurrentState == GameState.Paused)
        {
            cameraController.SetPause(false);
            CurrentState = GameState.Playing;
        }
    }

    public void NewGame()
    {
        if (CurrentState == GameState.Playing) return;
        Debug.Log("Starting NEEEEW game...");
        SceneManager.LoadScene("Scene_A");
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
    }

    public void Victory()
    {
        CurrentState = GameState.Victory;
    }
    
}
