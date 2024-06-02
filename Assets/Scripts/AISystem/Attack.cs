using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace ObjectPoolingSystem.AISystem
{
    public class Attack: AIState
    {
        private float rotationSpeed = 3.0f;
       // private AudioSource attack;
        public Attack(GameObject _zombie, NavMeshAgent _agent, Animator _anim, Transform _player) 
            : base(_zombie, _agent, _anim, _player)
        {
            name = AI_STATE.ATTACK;
            //attack = _zombie.GetComponent<AudioSource>();
        }

        public override void Enter()
        {
            anim.SetTrigger("attacking");
            agent.isStopped = true; // not sure rn, might not need to, so it actually walks while attack. maybe add slower speed only.
            base.Enter();
            player.GetComponent<IDamageable>()?.ApplyDamage(10);

        }
        public override void Update()
        {
            Vector3 direction = player.position - zombie.transform.position;
            float angle = Vector3.Angle(direction, zombie.transform.forward);
            direction.y = 0;

            zombie.transform.rotation = Quaternion.Slerp(zombie.transform.rotation, Quaternion.LookRotation(direction),
                Time.deltaTime * rotationSpeed);

            if (!CanAttackPlayer())
            {
                nextState = new Idle(zombie, agent, anim, player);
                stage = EVENT.EXIT;
            }
        }
        public override void Exit()
        {
            anim.ResetTrigger("attacking");
            base.Exit();
        }
    }
}