using System.Collections;
using ObjectPoolingSystem;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform barrelEnd;
    public float bulletSpeed = 1000f;
    public float spreadAngle = 5f;
    public GameObject muzzleFlashObject;
    public ObjectPool bulletPool;
    public AudioSource uziShootingAudio;

    public int maxAmmo = 60; 
    public int currentAmmo;
    public float reloadTime = 2.5f; 
    private bool isReloading = false;
    public Animator playerAnimator;

    private void Awake()
    {
        muzzleFlashObject.SetActive(false);
        uziShootingAudio.GetComponent<AudioSource>();
        currentAmmo = maxAmmo; 
        playerAnimator = GetComponentInParent<Animator>(); 

    }

    void Update()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            uziShootingAudio.Stop();
            muzzleFlashObject.SetActive(false);
            StartCoroutine(Reload());
            return;
        }
    }

    private IEnumerator Reload()
    {
        playerAnimator.SetBool("reload", true);
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        playerAnimator.SetBool("reload", false);
        isReloading = false;
    }

    public void ShootBullet()
    {
        if (isReloading || currentAmmo <= 0)
            return;

        currentAmmo--; 

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

        uziShootingAudio.Play();
        muzzleFlashObject.SetActive(true);
    }
    

    public void endAttack()
    {
        uziShootingAudio.Stop();
        muzzleFlashObject.SetActive(false);
    }
}
