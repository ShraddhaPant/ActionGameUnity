using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNPC : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float wanderTimer = 3f;

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDir = Random.insideUnitSphere * dist;
        randDir += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, dist, -1);

        return navHit.position;
    }
}