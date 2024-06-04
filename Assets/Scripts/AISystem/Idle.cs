using System.IO;
using AISystem;
using UnityEngine;
using UnityEngine.AI;

namespace ObjectPoolingSystem.AISystem
{
    public class Idle : AIState
    {
        public Idle(GameObject _zombie, NavMeshAgent _agent, Animator _anim, Transform _player) 
            : base(_zombie, _agent, _anim, _player)
        {
            name = AI_STATE.IDLE;
        }

        public override void Enter()
        {
            anim.SetTrigger("idle");
            base.Enter();
        }

        public override void Update()
        {
            if (CanSeePlayer())
            {
                nextState = new Chase(zombie, agent, anim, player);
                stage = EVENT.EXIT;
            }
            nextState = new Patrol(zombie, agent, anim, player);
            stage = EVENT.EXIT;
            
        }

        public override void Exit()
        {
            anim.ResetTrigger("idle");
            base.Exit();
        }
    }
}