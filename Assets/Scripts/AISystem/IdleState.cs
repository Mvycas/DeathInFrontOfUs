using UnityEngine;
using UnityEngine.AI;

namespace AISystem
{
    public class IdleState : AIState
    {
        private float rotationSpeed = 120f;
        private float idleDuration = 5f; 
        private float timer;

        public IdleState(AIController controller, NavMeshAgent agent) : base(controller, agent)
        {
        }

        public override void Enter()
        {
            agent.isStopped = true;
            // later on when I will connect animator:
            //controller.Animator.SetBool("IsIdle", true); 
            timer = idleDuration;
        }

        public override void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                controller.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                timer = idleDuration;
               
            }
        }
        
        public override void Exit()
        {
            agent.isStopped = false;
            //controller.Animator.SetBool("IsIdle", false); // later on
        }
    }
}