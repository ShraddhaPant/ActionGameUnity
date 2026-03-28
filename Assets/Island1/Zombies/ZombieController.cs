using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public Transform player;

    public float attackRange = 2f;
    public float detectionRange = 600f;
    public int attackDamage = 3;
    public float attackCooldown = 1.5f;
    public float moveSpeed = 3f;

    private Animator animator;
    private NavMeshAgent agent;

    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // 🔥 FORCE zombie onto NavMesh at start
        SnapToNavMesh();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.angularSpeed = 300f;
            agent.acceleration = 12f;
            agent.stoppingDistance = attackRange - 0.2f;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        // 🔥 ALWAYS ensure zombie stays on NavMesh
        if (!agent.isOnNavMesh)
        {
            SnapToNavMesh();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // 🧟 CHASE
        if (distance > attackRange && distance <= detectionRange)
        {
            agent.isStopped = false;

            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(player.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                // 🔥 FALLBACK movement if NavMesh path fails
                MoveDirectly();
            }

            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        // 🧟 ATTACK
        else if (distance <= attackRange)
        {
            agent.isStopped = true;

            // Look at player
            Vector3 lookPos = player.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);

            animator.SetFloat("Speed", 0f);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;

                animator.SetTrigger("Attack");

                PlayerHealth ph = player.GetComponentInParent<PlayerHealth>();
                if (ph != null)
                {
                    Debug.Log("🧟 Attacking Player!");
                    ph.TakeDamage(attackDamage);
                }
                else
                {
                    Debug.Log("❌ PlayerHealth NOT FOUND!");
                }
            }
        }

        // 🧟 IDLE
        else
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
        }
    }

    // 🔥 Snap zombie safely to nearest NavMesh
    void SnapToNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 50f, NavMesh.AllAreas))
        {
            transform.position = hit.position;

            if (agent != null)
                agent.Warp(hit.position);
        }
    }

    // 🔥 Backup movement if NavMesh fails
    void MoveDirectly()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        transform.position += dir * moveSpeed * Time.deltaTime;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    public void OnDeath()
    {
        isDead = true;

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;

        animator.SetTrigger("Die");

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        enabled = false;
    }
}