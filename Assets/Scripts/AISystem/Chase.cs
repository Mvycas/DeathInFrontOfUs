using ObjectPoolingSystem.AISystem;
using UnityEngine;
using UnityEngine.AI;

namespace AISystem
{
    public class Chase: AIState
    {
        public Chase(GameObject _zombie, NavMeshAgent _agent, Animator _anim, Transform _player) 
            : base(_zombie, _agent, _anim, _player)
        {
            name = AI_STATE.CHASE;
            agent.speed = 1;
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
            Vector3 direction = player.position - zombie.transform.position;
            float angle = Vector3.Angle(direction, zombie.transform.forward);
            direction.y = 0;
                
            zombie.transform.rotation = Quaternion.Slerp(zombie.transform.rotation, Quaternion.LookRotation(direction),
                Time.deltaTime * 2);
            
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