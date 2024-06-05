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
        private Rigidbody rb;
        private CapsuleCollider col;
        private NavMeshAgent agent;
        
        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
            ai = GetComponent<AIController>();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<CapsuleCollider>();
            agent = GetComponent<NavMeshAgent>();
        }

        protected override void Die()
        {
            if (col != null)
            {
                rb.isKinematic = true;  // stop physics interactions but keep the body in place
                rb.detectCollisions = false;
            }
            anim.SetBool("die", true);
            agent.enabled = false;
            ai.enabled = false;
            StartCoroutine(HandleDeath());
            
        }
        
        private IEnumerator HandleDeath()
        {
            yield return new WaitForSeconds(3f);
            //reset 
            ai.enabled = true;
            rb.isKinematic = false; 
            rb.detectCollisions = true;
            agent.enabled = true;
            gameObject.SetActive(false);
        }
    }
}

