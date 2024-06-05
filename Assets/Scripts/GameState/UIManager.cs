﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameState
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public GameObject mainMenuPanel;
        public GameObject gameOverPanel;
        public GameObject victoryPanel;

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
            }
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetupUI();
            UpdateUI(GameStateManager.Instance.CurrentState);
        }

        private void SetupUI()
        {
        
            // find panels 
            mainMenuPanel = GameObject.FindGameObjectWithTag("Menu_UI");
            gameOverPanel = GameObject.FindGameObjectWithTag("GameOverUI");
            victoryPanel = GameObject.FindGameObjectWithTag("VictoryPanel");

        
        
            GameObject[] newGameButtons = GameObject.FindGameObjectsWithTag("NewGameButton");
            foreach (GameObject buttonObject in newGameButtons)
            {
                Button newGameButton = buttonObject.GetComponent<Button>();
                if (newGameButton != null)
                {
                    newGameButton.onClick.RemoveAllListeners();
                    newGameButton.onClick.AddListener(() => GameStateManager.Instance.NewGame());
                }
            }
        
            GameObject[] exitGameButtons = GameObject.FindGameObjectsWithTag("ExitGameButton");
            foreach (GameObject buttonObject in exitGameButtons)
            {
                Button exitGameButton = buttonObject.GetComponent<Button>();
                if (exitGameButton != null)
                {
                    exitGameButton.onClick.RemoveAllListeners();
                    exitGameButton.onClick.AddListener(() => GameStateManager.Instance.ExitGame());
                }
            }
        
        }

        public void UpdateUI(GameStateManager.GameState state)
        {
            Debug.Log($"Updating UI to state: {state}");
            
            switch (state)
            {
                case GameStateManager.GameState.GameStart:
                    ActivatePanel(mainMenuPanel);
                    break;
                case GameStateManager.GameState.Playing:
                    DeactivateAllPanels();
                    break;
                case GameStateManager.GameState.Paused:
                    ActivatePanel(mainMenuPanel);
                    break;
                case GameStateManager.GameState.GameOver:
                    ActivatePanel(gameOverPanel);
                    break;
                case GameStateManager.GameState.Victory:
                    ActivatePanel(victoryPanel);
                    break;
            }
        }

        public void DeactivateAllPanels()
        {
            mainMenuPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            victoryPanel.SetActive(false);
        }

        private void ActivatePanel(GameObject panel)
        {
            DeactivateAllPanels();
            panel.SetActive(true);
        }
    }
}