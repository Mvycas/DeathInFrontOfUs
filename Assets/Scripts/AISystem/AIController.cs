using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;

namespace ObjectPoolingSystem.AISystem
{
    public class AIController : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Animator anim;
        private Transform player;
        private AIState currentState;

        void OnEnable() 
        {
            if (agent == null)
                agent = GetComponent<NavMeshAgent>();
            if (anim == null)
                anim = GetComponent<Animator>();
            player = GameObject.FindWithTag("Player").transform;

        }

        private void Start()
        {
            currentState = new Idle(this.gameObject, agent, anim, player);
        }

        private void Update()
        {
            currentState = currentState.Process();
        }
    }
}