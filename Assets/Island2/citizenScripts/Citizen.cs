using UnityEngine;
using UnityEngine.AI;

public class Citizen : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (anim == null)
        {
            Debug.LogError("Animator missing on " + gameObject.name);
        }
    }

    void Update()
    {
        if (anim == null || agent == null) return;

        if (agent.velocity.magnitude > 0.1f)
            anim.SetBool("isWalking", true);
        else
            anim.SetBool("isWalking", false);
    }

    public void Die()
    {
        if (agent != null)
            agent.isStopped = true;

        if (anim != null)
            anim.SetBool("isDead", true);
    }
}
