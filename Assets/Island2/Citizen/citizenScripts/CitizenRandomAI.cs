using UnityEngine;
using UnityEngine.AI;

public class CitizenRandomAI : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator; // ✅ added
    public float roamRadius = 200f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // ✅ added
        MoveToNewPosition();
    }


    void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;
        if (!agent.pathPending && agent.remainingDistance < 2f)
        {
            MoveToNewPosition();
        }

        // ✅ Animation control (added only)
        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
    }

    void MoveToNewPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection.y = 0; // IMPORTANT: keep on ground

        Vector3 targetPosition = transform.position + randomDirection;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, roamRadius, NavMesh.AllAreas))
        {
            if (Vector3.Distance(transform.position, hit.position) > 2f) // avoid same spot
            {
                agent.SetDestination(hit.position);
            }
        }
    }
}