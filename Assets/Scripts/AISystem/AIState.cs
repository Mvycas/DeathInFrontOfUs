using UnityEngine;
using UnityEngine.AI;

namespace AISystem
{
    public class AIState
    {
        public enum AI_STATE
        {
            IDLE,
            PATROL,
            CHASE,
            ATTACK
        }
        
        public enum EVENT
        {
            ENTER, UPDATE, EXIT
        }

        public AI_STATE name;
        protected EVENT stage;
        protected GameObject zombie;
        protected Animator anim;
        protected Transform player;
        protected AIState nextState;
        protected NavMeshAgent agent;

        //zombie:
        private float visualDistance = 25.0f;
        private float visualAngle = 180.0f; //Yes my zombies have wide angle eyes ;))))
        private float attackDist = 1.0f;

        public AIState(GameObject _zombie, NavMeshAgent _agent, Animator _anim, Transform _player)
        {
            zombie = _zombie;
            agent = _agent;
            anim = _anim;
            stage = EVENT.ENTER;
            player = _player;
        }

        public virtual void Enter()
        {
            stage = EVENT.UPDATE;
        }

        public virtual void Update()
        {
            stage = EVENT.UPDATE;
        }

        public virtual void Exit()
        {
            stage = EVENT.EXIT;
        }

        public AIState Process()
        {
            if (stage == EVENT.ENTER) Enter();
            if (stage == EVENT.UPDATE) Update();
            if (stage == EVENT.EXIT)
            {
                Exit();
                return nextState;
            }
            return this;
        }

        public bool CanSeePlayer()
        {
            Vector3 direction = player.position - zombie.transform.position;
            float angle = Vector3.Angle(direction, zombie.transform.forward);
            
            if (direction.magnitude < visualDistance && angle < visualAngle)
            {
                return true;
            }
            return false;
        }

        public bool CanAttackPlayer()
        {
            Vector3 direction = player.position - zombie.transform.position;
            if (direction.magnitude < attackDist)
            {
                return true;
            }
            return false;
        }
    }
}