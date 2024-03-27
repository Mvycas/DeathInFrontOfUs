using UnityEngine;

public class ZombieSpawnManager : MonoBehaviour
{
    public ObjectPool zombiePool; // Reference zombie obj pool
    public Transform[] spawnPoints; // Array of potential spawn points for zombies
    public int maxZombies = 50; // The maximum number of zombies allowed at once

    private void Update()
    {
        // Spawn new zombies if the count of active ones is less than the maximum allowed
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
            // Choose a random spawn point to place the zombie
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            zombie.transform.position = spawnPoint.position;
            zombie.transform.rotation = spawnPoint.rotation;

            // Activate the zombie
            zombie.SetActive(true);
        }
    }
}