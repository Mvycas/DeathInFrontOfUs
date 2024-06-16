using System;
using System.Collections;
using GameState;
using MovementSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HealthSystem
{
    public class Player : Character
    {
        private Animator anim; 
        public Movement movement; 

        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
            movement = GetComponent<Movement>();
        }

        protected override void Die()
        {
            if (anim != null)
            {
                GetComponentInChildren<TakeDamage>().OnPlayerDeath();
                StartCoroutine(HandleDeath());
            }

        }
        private IEnumerator HandleDeath()
        {
            movement.enabled = false;
            anim.SetBool("death", true);
            yield return new WaitForSeconds(3.5f);
            // death animation should be finished in 3 sec, call gameOver :(
            GameStateManager.Instance.GameOver();
        }
    }
}