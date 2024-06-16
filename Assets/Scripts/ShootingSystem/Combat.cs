using InputSystem;
using UnityEngine;

namespace ShootingSystem
{
    [RequireComponent(typeof(PlayerInputConfig))]
    [RequireComponent(typeof(Animator))]
    public class Combat : MonoBehaviour
    {

        private PlayerInputConfig _playerInput;

        public Gun gun;
        public float timeBetweenShots = 0.01f; // Adjust this for shot rate
        private float _timestamp;
        


        public bool AttackInProgress {get; private set;} = false;

        private void Start()
        {
            _playerInput = GetComponent<PlayerInputConfig>();

        }

        private void Update()
        {
            if (_playerInput.AttackInput && Time.time >= _timestamp)
            {
                if (!AttackInProgress)
                {
                    SetAttackStart();
                }
                Attack();
            }
            else if (_playerInput.AttackInput == false && AttackInProgress)
            {
                SetAttackEnd();
            }
        }

        private void SetAttackStart()
        {
            AttackInProgress = true;
        }

        private void Attack()
        {
            
            //gun.muzzleFlashObject.SetActive(true);
            //gun.uziShootingAudio.Play();
            gun.ShootBullet();
            _timestamp = Time.time + timeBetweenShots;
        }
        
        private void SetAttackEnd()
        {
            gun.endAttack();
            AttackInProgress = false;
            _timestamp = 0; 
        }
    }
}