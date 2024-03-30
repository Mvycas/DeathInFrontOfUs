using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectPoolingSystem
{
public class ObjectPool : MonoBehaviour
{
    public GameObject prefabToPool;
    public int poolSize = 20;
    private List<GameObject> _pooledObjects;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        _pooledObjects = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefabToPool);
            obj.SetActive(false);
            _pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        return _pooledObjects.FirstOrDefault(t => !t.activeInHierarchy);
    }
    
    public int CountActiveObjects()
    {
        return _pooledObjects.Count(obj => obj.activeInHierarchy);
    }
    
}
}