using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracking : MonoBehaviour
{
    public string playerTag = "Player";
    private Transform player;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        FindPlayer();
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position;
        }
    }
    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        FindPlayer();
    }
}
