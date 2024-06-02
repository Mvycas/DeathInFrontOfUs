using UnityEngine;
using UnityEngine.AI;
using System;


namespace ObjectPoolingSystem.AISystem
{
    public class Patrol: AIState
    {
        private int currentIndex = -1;
        private System.Random random = new System.Random();

        public Patrol(GameObject _zombie, NavMeshAgent _agent, Animator _anim, Transform _player) 
            : base(_zombie, _agent, _anim, _player)
        {
            name = AI_STATE.PATROL;
            agent.speed = 1;
            agent.isStopped = false;
        }

        public override void Enter()
        {
            currentIndex = 0;
            anim.SetTrigger("walking");
            base.Enter();
        }
        public override void Update()
        {
            if (agent.remainingDistance < 3)
            {
                int newIndex;
                do
                {
                    newIndex = random.Next(GameEnvironment.Singleton.Waypoints.Count);
                } while (newIndex == currentIndex); 

                currentIndex = newIndex;
                agent.SetDestination(GameEnvironment.Singleton.Waypoints[currentIndex].transform.position);
            }
            if (CanSeePlayer())
            {
                nextState = new Chase(zombie, agent, anim, player);
                stage = EVENT.EXIT;
            }
        }
        public override void Exit()
        {
            anim.ResetTrigger("walking");
            base.Exit();
        }
    }
}