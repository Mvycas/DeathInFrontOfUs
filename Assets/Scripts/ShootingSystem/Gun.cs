using UnityEngine;
using ObjectPoolingSystem;

namespace ShootingSystem
{
    public class Gun : MonoBehaviour
    {
        public Transform barrelEnd; 
        public float bulletSpeed = 1000f;
        public float spreadAngle = 5f; 
        public GameObject muzzleFlashObject; 
        public ObjectPool bulletPool;
        public AudioSource uziShootingAudio;

        private void Awake()
        {
            muzzleFlashObject.SetActive(false);
            uziShootingAudio.GetComponent<AudioSource>();
        }
        
        public void ShootBullet()
        {
            GameObject bullet = bulletPool.GetPooledObject();
            if (bullet == null) return;
            
            Vector3 shootDirection = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle), // Pitch
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