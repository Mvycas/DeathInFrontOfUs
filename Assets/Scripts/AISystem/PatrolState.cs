using AISystem;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : AIState
{
    private Transform[] waypoints;
    private int currentWaypointIndex;

    public PatrolState(AIController controller, NavMeshAgent agent, Transform[] waypoints) : base(controller, agent)
    {
        this.waypoints = waypoints; 
        currentWaypointIndex = 0; 
    }

    public override void Enter()
    {
        agent.speed = controller.speedWalk;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    public override void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    public override void Exit() { }
}