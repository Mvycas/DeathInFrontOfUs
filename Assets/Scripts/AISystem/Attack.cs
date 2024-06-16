using Interfaces;
using ObjectPoolingSystem.AISystem;
using UnityEngine;
using UnityEngine.AI;

namespace AISystem
{
    public class Attack: AIState
    {
        private float rotationSpeed = 2;
        private float attackCooldown = 0.5f;
        private float lastAttackTime;
       // private AudioSource attack;
        public Attack(GameObject _zombie, NavMeshAgent _agent, Animator _anim, Transform _player) 
            : base(_zombie, _agent, _anim, _player)
        {
            name = AI_STATE.ATTACK;
            //attack = _zombie.GetComponent<AudioSource>();
        }

        public override void Enter()
        {
            lastAttackTime = Time.time;
            anim.SetTrigger("attacking");
            agent.isStopped = true; 
            base.Enter();
        }
        
        public override void Update()
        {
            Vector3 direction = player.position - zombie.transform.position;
            direction.y = 0; 

            zombie.transform.rotation = Quaternion.Slerp(zombie.transform.rotation, Quaternion.LookRotation(direction),
                Time.deltaTime * rotationSpeed);
            
            if (!CanAttackPlayer())
            {
                nextState = new Idle(zombie, agent, anim, player);
                stage = EVENT.EXIT;
            }
            else
            {
                if (!(Time.time - lastAttackTime >= attackCooldown)) return;
                
                Debug.Log($"Attacking at {Time.time}, Damage Applied");
                
                IDamageable damageable = player.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(10);
                    
                    TakeDamage takeDamageScript = player.GetComponentInChildren<TakeDamage>();
                    if (takeDamageScript != null)
                    {
                        takeDamageScript.ApplyDamageEffect();
                    }
                    else
                    {
                        Debug.LogWarning("TakeDamage component not found on the player!");
                    }
                }

                lastAttackTime = Time.time;
            }
        }

        public override void Exit()
        {
            anim.ResetTrigger("attacking");
            base.Exit();
        }
    }
}