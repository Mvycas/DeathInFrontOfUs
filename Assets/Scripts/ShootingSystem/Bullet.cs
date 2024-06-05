using HealthSystem;
using UnityEngine;

namespace ShootingSystem
{
    public class Bullet : MonoBehaviour
    {
        public float maxTravelDistance = 100f; // Maximum distance the bullet can travel
        private Vector3 _spawnPosition; // Position where the bullet is spawned
        public float damageAmount = 10f; // The damage the bullet will deal
        public GameObject goreEffectPrefab; // Assign your gore effect prefab in the inspector
        public float lifetime = 5f; // How long the bullet lives before getting disabled

        private float _timeSinceActivated; // Keeps track of how long the bullet has been active

        private void OnEnable()
        {
            _timeSinceActivated = 0f; // Reset the timer when the bullet is enabled
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
            // Only get the Character component if the collision object has the correct tag
            if (collision.gameObject.CompareTag("Zombie"))
            {
                var character = collision.collider.GetComponent<Character>();
                if (character != null)
                {
                    character.ApplyDamage(damageAmount);
                }
                Vector3 effectPosition = collision.contacts[0].point - collision.contacts[0].normal * 0.1f;  

                // Instantiate the gore effect at the collision point and orient it correctly
                var goreEffectInstance = Instantiate(goreEffectPrefab, effectPosition, Quaternion.LookRotation(collision.contacts[0].normal));
                goreEffectInstance.transform.SetParent(collision.transform);
                goreEffectInstance.transform.localPosition = collision.transform.InverseTransformPoint(collision.contacts[0].point);
                var ps = goreEffectInstance.GetComponent<ParticleSystem>();
                ps.Play();

                // Destroy the gore effect after it has finished
                Destroy(goreEffectInstance, ps.main.duration);
            }
            // Deactivate the bullet in any case
            DisableBullet();
        }

        private void DisableBullet()
        {
            gameObject.SetActive(false);
        }
    }
}