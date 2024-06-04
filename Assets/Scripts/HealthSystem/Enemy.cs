using System;
using System.Collections;
using AISystem;
using UnityEngine;
using UnityEngine.AI;

namespace HealthSystem
{
    public class Enemy : Character
    {
        private Animator anim;
        private AIController ai;  // Reference to AI manager

        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
            ai = GetComponent<AIController>();
        }

        protected override void Die()
        {
            anim.SetBool("die", true);
            GetComponent<NavMeshAgent>().enabled = false;
            ai.enabled = false;
            StartCoroutine(HandleDeath());
            
        }
        
        private IEnumerator HandleDeath()
        {
            yield return new WaitForSeconds(5f);
            ai.enabled = true;
            GetComponent<NavMeshAgent>().enabled = true;
            gameObject.SetActive(false);
        }
    }
}

