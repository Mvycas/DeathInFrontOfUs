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
            // Store the initial spawn position of the bullet
            _spawnPosition = transform.position;
        }

        private void Update()
        {
            // Update the timer
            _timeSinceActivated += Time.deltaTime;

            // Check if the bullet has existed longer than its lifetime
            if (_timeSinceActivated >= lifetime)
            {
                DisableBullet();
            }
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        // Get the character component from the collided object
        // If character is detected, give damage // should be enemy later on
        //  Character character = collision.collider.GetComponent<Character>();
        //  if (character != null)
        //  {
        // Apply damage to the character
        //    character.ApplyDamage(damageAmount);
        //  }

        // Destroy the bullet on collision 
        // Not sure, but this supposed to destroy bullet on collision event,
        // so if it collides with another object that has rigidbody.
        // Of course if it collides with character, it applies the damage first (as you can see in the above code)
        // Ask Jakob if it is fine this way. Not sure of code quality ... hh
        //  Destroy(gameObject);
        // }
   
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

                // Instantiate the gore effect at the collision point and orient it correctly
                var goreEffectInstance = Instantiate(goreEffectPrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
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
            // Deactivate the bullet
            gameObject.SetActive(false);
        }
    }
}