using System;
using System.Collections;
using GameState;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HealthSystem
{
    public class Player : Character
    {
        private Animator anim; 
        private PlayerInput input;

        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
            input = GetComponent<PlayerInput>();
        }

        protected override void Die()
        {
            Debug.Log("Player died!");
            if (anim != null)
            {
                StartCoroutine(HandleDeathAnimation());
            }

        }
        private IEnumerator HandleDeathAnimation()
        {
            input.enabled = false; 
            anim.SetBool("death", true);

            yield return new WaitForSeconds(3f);
            // death animation should be finished in 3 sec, call gameOver :(
            GameStateManager.Instance.GameOver();
        }
    }
}