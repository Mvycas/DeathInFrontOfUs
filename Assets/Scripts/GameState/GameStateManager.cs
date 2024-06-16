using System.Collections;
using Lighting;
using MovementSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameState
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }
        private PlayerController _playerController;  
        private GameState _currentState;
        private CameraController _cameraController;
        private bool _gameStarted;

        public enum GameState
        {
            GameStart,
            Playing,
            Paused,
            GameOver,
            Victory
        }

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
            }
        }
    
        void OnEnable() {
            SceneManager.sceneLoaded -= OnSceneLoaded; 
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;  
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (_cameraController == null) {
                _cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
            }
            
            _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            
            if (!_gameStarted) {
                _gameStarted = true;  
                _cameraController.SetPause(true);
                CurrentState = GameState.GameStart;
            } else {
                CurrentState = GameState.Playing;
                LightManager.Instance.StartSunDimming();  
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
                    Time.timeScale = 0; 
                    _playerController.EnableControls(false);
                    UIManager.Instance.UpdateUI(state);
                    break;
                case GameState.Playing:
                    Time.timeScale = 1; 
                    _playerController.EnableControls(true);
                    UIManager.Instance.UpdateUI(state);
                    break;
                case GameState.Paused:
                    Time.timeScale = 0; 
                    _playerController.EnableControls(false);
                    UIManager.Instance.UpdateUI(state);
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0; 
                    _playerController.EnableControls(false);
                    UIManager.Instance.UpdateUI(state);
                    break;
                case GameState.Victory:
                    Time.timeScale = 0; 
                    _playerController.EnableControls(false);
                    UIManager.Instance.UpdateUI(state);
                    break;
            }
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Playing)
            {
                _cameraController.SetPause(true);
                CurrentState = GameState.Paused;
            }
            else if (CurrentState == GameState.Paused)
            {
                _cameraController.SetPause(false);
                CurrentState = GameState.Playing;
            }
        }

        public void NewGame()
        {
            if (CurrentState == GameState.Playing) return;
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
    
        public void ExitGame()
        {
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
