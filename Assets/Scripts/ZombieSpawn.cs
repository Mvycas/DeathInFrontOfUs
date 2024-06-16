using UnityEngine;
using ObjectPoolingSystem;

public class ZombieSpawnManager : MonoBehaviour
{
    public ObjectPool zombiePool;
    private Transform[] spawnPoints;
    public int maxZombies = 50;

    private void Start()
    {
        spawnPoints = GameEnvironment.Singleton.SpawnPoints.ToArray();
    }

    private void Update()
    {
        int activeZombiesCount = zombiePool.CountActiveObjects();
        if (activeZombiesCount < maxZombies)
        {
            SpawnZombie();
        }
    }

    private void SpawnZombie()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        GameObject zombie = zombiePool.GetPooledObject();
        if (zombie != null)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            zombie.transform.position = spawnPoint.position;
            zombie.transform.rotation = spawnPoint.rotation;
            zombie.SetActive(true);
        }
    }
}