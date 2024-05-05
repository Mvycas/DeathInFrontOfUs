using UnityEngine.AI;

namespace AISystem
{
    public abstract class AIState
    {
        protected AIController controller;
        protected NavMeshAgent agent;

        protected AIState(AIController controller, NavMeshAgent agent)
        {
            this.controller = controller;
            this.agent = agent;
        }

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}