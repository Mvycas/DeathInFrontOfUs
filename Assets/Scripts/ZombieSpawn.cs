using UnityEngine;
using ObjectPoolingSystem;

public class ZombieSpawnManager : MonoBehaviour
{
    public ObjectPool zombiePool; 
    public Transform[] spawnPoints; 
    public int maxZombies = 50; 

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