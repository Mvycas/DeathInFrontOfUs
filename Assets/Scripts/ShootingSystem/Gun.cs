using UnityEngine;
using ObjectPoolingSystem;

namespace ShootingSystem
{
    public class Gun : MonoBehaviour
    {
        public Transform barrelEnd; 
        public float bulletSpeed = 1000f;
        public float spreadAngle = 5f; // Degrees of spread
        public GameObject muzzleFlashObject; // Assign muzzle flash in the editor
        public ObjectPool bulletPool; 

        private void Awake()
        {
            // Ensure the muzzle flash is disabled on start
            muzzleFlashObject.SetActive(false);
        }
        
        public void ShootBullet()
        {
            GameObject bullet = bulletPool.GetPooledObject();
            if (bullet == null) return;
            
            Vector3 shootDirection = Quaternion.Euler(
                0, // Pitch
                Random.Range(-spreadAngle, spreadAngle), // Yaw
                0 // Roll is typically not needed for bullets
            ) * barrelEnd.forward;

            bullet.transform.position = barrelEnd.position;
        
            bullet.SetActive(true); 

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero; 
            rb.AddForce(shootDirection * bulletSpeed, ForceMode.VelocityChange);
            
        }

    }
}