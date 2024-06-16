using HealthSystem;
using UnityEngine;

namespace ShootingSystem
{
    public class Bullet : MonoBehaviour
    {
        public float maxTravelDistance = 150f; 
        private Vector3 _spawnPosition; 
        public float damageAmount = 10f; 
        public GameObject goreEffectPrefab; 
        public float lifetime = 5f; 

        private float _timeSinceActivated; 

        private void OnEnable()
        {
            _timeSinceActivated = 0f; 
        }
        private void Start()
        {
            _spawnPosition = transform.position;
        }

        private void Update()
        {
            _timeSinceActivated += Time.deltaTime;

            if (_timeSinceActivated >= lifetime)
            {
                DisableBullet();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            DisableBullet();
        }

        private void DisableBullet()
        {
            gameObject.SetActive(false);
        }
    }
}