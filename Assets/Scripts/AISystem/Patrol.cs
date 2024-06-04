using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using AISystem;

namespace ObjectPoolingSystem.AISystem
{
    public class Patrol : AIState
    {
        private int currentIndex = -1;
        private System.Random random = new System.Random();
        private List<int> availableWaypoints;

        public Patrol(GameObject _zombie, NavMeshAgent _agent, Animator _anim, Transform _player) 
            : base(_zombie, _agent, _anim, _player)
        {
            name = AI_STATE.PATROL;
            agent.speed = 0.5f;
            agent.isStopped = false;

            // init the list of available waypoints, excluding the current index
            InitializeAvailableWaypoints();
        }

        private void InitializeAvailableWaypoints()
        {
            availableWaypoints = new List<int>();
            for (int i = 0; i < GameEnvironment.Singleton.Waypoints.Count; i++)
            {
                if (i != currentIndex)
                {
                    availableWaypoints.Add(i);
                }
            }
        }

        public override void Enter()
        {
            SelectNewWaypoint();
            anim.SetTrigger("walking");
            base.Enter();
        }

        public override void Update()
        {
            if (agent.remainingDistance < 5)
            {
                SelectNewWaypoint();
            }

            if (CanSeePlayer())
            {
                nextState = new Chase(zombie, agent, anim, player);
                stage = EVENT.EXIT;
            }
        }

        private void SelectNewWaypoint()
        {
            if (availableWaypoints.Count > 0)
            {
                int waypointIndex = random.Next(availableWaypoints.Count);
                currentIndex = availableWaypoints[waypointIndex];
                agent.SetDestination(GameEnvironment.Singleton.Waypoints[currentIndex].transform.position);

                // re-init available waypoints excluding the newly selected current index
                InitializeAvailableWaypoints();
            }
            else
            {
                Debug.LogError("No available waypoints");
            }
        }

        public override void Exit()
        {
            anim.ResetTrigger("walking");
            base.Exit();
        }
    }
}
