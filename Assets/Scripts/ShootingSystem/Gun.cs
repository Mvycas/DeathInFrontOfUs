using UnityEngine;

namespace ShootingSystem
{
    public class Gun : MonoBehaviour
    {
        public Transform barrelEnd; 
        public float bulletSpeed = 1000f;
        public float spreadAngle = 5f; // Degrees of spread
        public GameObject muzzleFlashObject; // Assign muzzle flash in the editor
    
        private void Awake()
        {
            // Ensure the muzzle flash is disabled on start
            muzzleFlashObject.SetActive(false);
        }
        
        public void ShootBullet()
        {
            GameObject bullet = BulletPool.sharedInstance.GetPooledBullet();
            if (bullet == null) return;
            // Calculate random spread for the shoot direction
            Vector3 shootDirection = Quaternion.Euler(
                0, // Pitch
                Random.Range(-spreadAngle, spreadAngle), // Yaw
                0 // Roll is typically not needed for bullets
            ) * barrelEnd.forward;

            bullet.transform.position = barrelEnd.position; // Set the bullet's position
            //bullet.transform.rotation = Quaternion.LookRotation(shootDirection); // Set the bullet's rotation
        
            bullet.SetActive(true); // Activate the bullet

            // Apply force to the bullet Rigidbody
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero; // Reset the bullet's velocity before applying a new force
            //rb.interpolation = RigidbodyInterpolation.Interpolate; // Ensure smooth motion
            rb.AddForce(shootDirection * bulletSpeed, ForceMode.VelocityChange);
        
            // Optional: Debug line to visualize the shooting direction
            // Debug.DrawRay(barrelEnd.position, shootDirection * 10, Color.red, 2f);
        }

//        public void ShootBullet()
//        {
//               // Instantiate the bullet at the barrel end position and rotation
//            GameObject bullet = Instantiate(bulletPrefab, barrelEnd.position, Quaternion.identity);

               // Calculate direction from the gun to the target
               // Calculate random spread for the shoot direction
//            Vector3 shootDirection = Quaternion.Euler(
                //Random.Range(-spreadAngle, spreadAngle), // Pitch
//                0,
//                Random.Range(-spreadAngle, spreadAngle), // Yaw
//                0 // Roll is typically not needed for bullets
//            ) * barrelEnd.forward;

                // Apply force to the bullet Rigidbody
//            Rigidbody rb = bullet.GetComponent<Rigidbody>();
//            rb.AddForce(shootDirection * bulletSpeed);

            //Debug.DrawRay(barrelEnd.position, shootDirection * 10, Color.red, 2f);

//        }
    }
}