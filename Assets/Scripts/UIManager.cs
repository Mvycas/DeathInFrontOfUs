using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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


        // find buttons
        Button newGameButton = GameObject.FindGameObjectWithTag("NewGameButton").GetComponent<Button>();
        newGameButton.onClick.RemoveAllListeners(); // to avoid duplicates
        newGameButton.onClick.AddListener(() => GameStateManager.Instance.NewGame());
        
    }

    public void UpdateUI(GameStateManager.GameState state)
    {
        Debug.Log($"Updating UI to state: {state}");

        switch (state)
        {
            case GameStateManager.GameState.GameStart:
                Debug.Log("UI Manager: game start state");
                ActivatePanel(mainMenuPanel);
                break;
            case GameStateManager.GameState.Playing:
                Debug.Log("Deactivating all panels for Playing state");
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
        //mainMenuPanel.SetActive(false);
       // gameOverPanel.SetActive(false);
        //victoryPanel.SetActive(false);
    }

    private void ActivatePanel(GameObject panel)
    {
        DeactivateAllPanels();
        panel.SetActive(true);
    }
}