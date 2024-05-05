using UnityEngine.AI;

namespace AISystem
{
    public class ChaseState : AIState
    {
        public ChaseState(AIController controller, NavMeshAgent agent) : base(controller, agent) { }

        public override void Enter()
        {
            agent.speed = controller.speedRun;
        }

        public override void Update()
        {
            agent.SetDestination(controller.lastKnownPlayerPosition);
        }

        public override void Exit() { }
    }
}