using UnityEngine;
using UnityEngine.AI;

public class CitizenRandomAI : MonoBehaviour
{
    NavMeshAgent agent;
    public float roamRadius = 10f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        MoveToNewPosition();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 2f)
        {
            MoveToNewPosition();
        }
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