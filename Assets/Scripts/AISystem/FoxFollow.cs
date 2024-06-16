using UnityEngine;
using UnityEngine.AI;

public class FoxFollow : MonoBehaviour
{
    public Transform player;
    public float stoppingDistance = 10.0f; // Distance at which the fox should stop from the player
    private NavMeshAgent agent; 
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.stoppingDistance = stoppingDistance;
        agent.autoBraking = true;
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);

            // Check the distance to the target
            bool isCloseEnough = agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;

            // Manage movement and animations based on distance
            if (isCloseEnough)
            {
                agent.isStopped = true; // Stop the agent from moving
                animator.SetBool("running", false); // Transition to idle animation
                animator.SetBool("chill", true); // Transition to idle animation
            }
            else
            {
                agent.isStopped = false; // Allow the agent to move
                animator.SetBool("chill", false); // Transition to idle animation
                animator.SetBool("running", true); // Transition to running animation
            }
        }
    }
}