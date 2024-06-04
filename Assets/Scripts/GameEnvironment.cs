using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameEnvironment : MonoBehaviour
{
    private static GameEnvironment instance;
    private List<GameObject> waypoints = new List<GameObject>();
    private List<Transform> spawnPoints = new List<Transform>();

    public List<GameObject> Waypoints
    {
        get
        {
            if (waypoints == null || waypoints.Count == 0)
            {
                RefreshWaypoints();
            }
            return waypoints;
        }
    }
    
    public List<Transform> SpawnPoints
    {
        get
        {
            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                RefreshSpawnPoints();
            }
            return spawnPoints;
        }
    }

    public static GameEnvironment Singleton
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject("GameEnvironment");
                instance = obj.AddComponent<GameEnvironment>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshWaypoints();
        RefreshSpawnPoints();

    }

    private void RefreshWaypoints()
    {
        waypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Waypoint"));
    }
    private void RefreshSpawnPoints()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("SpawnPoints");
        spawnPoints = new List<Transform>();
        foreach (var point in points)
        {
            spawnPoints.Add(point.transform);
        }
    }
}