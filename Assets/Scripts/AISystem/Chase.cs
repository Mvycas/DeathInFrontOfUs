using System.IO.Pipes;
using UnityEngine;
using UnityEngine.AI;

namespace ObjectPoolingSystem.AISystem
{
    public class Chase: AIState
    {
        public Chase(GameObject _zombie, NavMeshAgent _agent, Animator _anim, Transform _player) 
            : base(_zombie, _agent, _anim, _player)
        {
            name = AI_STATE.CHASE;
            agent.speed = 5;
            agent.isStopped = false;
        }

        public override void Enter()
        {
            anim.SetTrigger("running");
            base.Enter();
        }

        public override void Update()
        {
            agent.SetDestination(player.position);
            if (agent.hasPath)
            {
                if (CanAttackPlayer())
                {
                    nextState = new Attack(zombie, agent, anim, player);
                    stage = EVENT.EXIT;
                }
                else if (!CanSeePlayer())
                {
                    nextState = new Patrol(zombie, agent, anim, player);
                    stage = EVENT.EXIT;
                }
            }
        }

        public override void Exit()
        {
            anim.ResetTrigger("running");
            base.Exit();
        }
    }
}