using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShootingSystem
{
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool sharedInstance;
        public List<GameObject> pooledBullets;
        public GameObject bulletPrefab;
        public int bulletAmount = 20;

        private void Awake()
        {
            sharedInstance = this;
            pooledBullets = new List<GameObject>();
            for (var i = 0; i < bulletAmount; i++)
            {
                var tmp = Instantiate(bulletPrefab);
                tmp.SetActive(false);
                pooledBullets.Add(tmp);
            }
        }

        public GameObject GetPooledBullet()
        {
            return pooledBullets.FirstOrDefault(t => !t.activeInHierarchy);
        }
    }
}