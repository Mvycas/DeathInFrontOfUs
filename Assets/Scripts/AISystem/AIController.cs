using System;
using UnityEngine;
using UnityEngine.AI;

namespace AISystem
{
    public class AIController : MonoBehaviour {
        public NavMeshAgent navMeshAgent;
        public float speedWalk = 3.5f;
        public float speedRun = 7f;
        [HideInInspector]
        public Vector3 lastKnownPlayerPosition;

        private StateMachine<AIState> stateMachine;
        private PatrolPath patrolPath;
        public Transform[] waypoints;
        
        private void Awake()
        {
            patrolPath = FindObjectOfType<PatrolPath>(); 
            if (patrolPath != null)
            {
                waypoints = patrolPath.GetWaypoints(); 
            }
            stateMachine = new StateMachine<AIState>();
        }
        private void OnDisable()
        {
            stateMachine?.CurrentState.Exit(); 
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            navMeshAgent.enabled = false; 
        }

        private void OnEnable()
        {
            navMeshAgent.enabled = true; 
            navMeshAgent.isStopped = false;
            InitializeStateMachine(); 
        }

        private void Start()
        {
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            stateMachine = new StateMachine<AIState>();
            if (waypoints != null && waypoints.Length > 0)
            {
                stateMachine.Initialize(new PatrolState(this, navMeshAgent, waypoints));
            }
            else
            {
                Debug.Log("No waypoints found!!!!!");
                stateMachine.Initialize(new IdleState(this, navMeshAgent));
            }
        }


        private void Update()
        {
            stateMachine.CurrentState.Update();
        }

        public void SeePlayer(Vector3 playerPosition)
        {
            lastKnownPlayerPosition = playerPosition;
            if (!(stateMachine.CurrentState is ChaseState))
            {
                stateMachine.ChangeState(new ChaseState(this, navMeshAgent));
            }
        }

        public void LosePlayer()
        {
            if (!(stateMachine.CurrentState is PatrolState) && waypoints != null && waypoints.Length > 0)
            {
                stateMachine.ChangeState(new PatrolState(this, navMeshAgent, waypoints));
            }
            else
            {
                // Go back to IdleState
                stateMachine.ChangeState(new IdleState(this, navMeshAgent));
            }
        }
    }
}
