using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class DragonAI : MonoBehaviour
{
    [Header("Detection & Attack Range")]
    public float attackRange = 8f;

    [Header("Movement")]
    public float chaseSpeed = 5f;

    [Header("Combat")]
    public float attackCooldown = 3f;

    [Header("AI Tick")]
    public float minDecisionDelay = 0.2f;
    public float maxDecisionDelay = 0.5f;

    [Header("Death")]
    public float disappearDelay = 3f;

    private Animator anim;
    private NavMeshAgent agent;

    private Transform npcTarget;
    private float attackTimer = 0f;
    private bool isDead = false;
    private bool isAttacking = false;

    private int hIdleSimple, hIdleAgressive, hIdleRestless;
    private int hWalk, hBattleStance, hBite, hDrakaris;
    private int hFlyingFWD, hFlyingAttack, hHover;
    private int hLands, hTakeOff, hDie;

    private string lastLog = "";

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // ✅ FIX 1: Prevent overlap with player/NPC
        agent.stoppingDistance = attackRange * 0.8f;

        // ✅ FIX 2: Ensure no physics conflict
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // ✅ FIX 3: Ignore collision with Player (prevents sinking)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider dragonCol = GetComponent<Collider>();
            Collider playerCol = player.GetComponent<Collider>();

            if (dragonCol != null && playerCol != null)
            {
                Physics.IgnoreCollision(dragonCol, playerCol);
            }
        }

        hIdleSimple = Animator.StringToHash("IdleSimple");
        hIdleAgressive = Animator.StringToHash("IdleAgressive");
        hIdleRestless = Animator.StringToHash("IdleRestless");
        hWalk = Animator.StringToHash("Walk");
        hBattleStance = Animator.StringToHash("BattleStance");
        hBite = Animator.StringToHash("Bite");
        hDrakaris = Animator.StringToHash("Drakaris");
        hFlyingFWD = Animator.StringToHash("FlyingFWD");
        hFlyingAttack = Animator.StringToHash("FlyingAttack");
        hHover = Animator.StringToHash("Hover");
        hLands = Animator.StringToHash("Lands");
        hTakeOff = Animator.StringToHash("TakeOff");
        hDie = Animator.StringToHash("Die");

        SetAllFalse();
        SetOnly(hIdleRestless);

        Log("🐉 Dragon awakens... scanning for targets.");
        StartCoroutine(AILoop());
    }

    void Update()
    {
        if (isDead) return;
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
    }

    IEnumerator AILoop()
    {
        while (!isDead)
        {
            if (isDead) yield break; // ✅ FIX: hard exit if death triggered mid-loop

            FindNPC();

            if (npcTarget == null)
                DragonWins();
            else
                DecideAction();

            yield return new WaitForSeconds(Random.Range(minDecisionDelay, maxDecisionDelay));
        }
    }

    void FindNPC()
    {
        if (isDead) return; // ✅ FIX: don't search if dead
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        if (npcs.Length == 0) { npcTarget = null; return; }

        Transform closest = null;
        float best = Mathf.Infinity;

        foreach (var go in npcs)
        {
            if (go.transform.position.y < -500f) continue;
            float d = Vector3.Distance(transform.position, go.transform.position);
            if (d < best) { best = d; closest = go.transform; }
        }

        npcTarget = closest;
    }

    void DecideAction()
    {
        if (isDead || isAttacking || npcTarget == null) return; // ✅ FIX: added isDead

        float dist = FlatDist(transform.position, npcTarget.position);

        if (dist <= attackRange && attackTimer <= 0f)
        {
            Log($"🐉 TARGET IN RANGE — attacking [{npcTarget.name}] (dist: {dist:F1})");
            StartCoroutine(AttackSequence());
        }
        else
        {
            Log($"🐉 CHASING [{npcTarget.name}] — distance: {dist:F1}");
            ChaseNPC();
        }
    }

    void ChaseNPC()
    {
        if (npcTarget == null || isDead) return; // ✅ FIX: don't chase if dead
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(npcTarget.position);
        FaceTarget(npcTarget);
        SetOnly(hFlyingFWD);
    }

    IEnumerator AttackSequence()
{
    isAttacking = true;
    attackTimer = attackCooldown;

    agent.isStopped = true;
    agent.ResetPath();

    Transform victim = npcTarget;
    npcTarget = null;

    if (victim == null)
    {
        Debug.LogWarning("[DragonAI] ⚠️ Victim disappeared before attack.");
        isAttacking = false;
        yield break;
    }

    NavMeshAgent victimNav = victim.GetComponent<NavMeshAgent>();
    Rigidbody victimRb = victim.GetComponent<Rigidbody>();

    if (victimNav != null) { victimNav.isStopped = true; victimNav.enabled = false; }
    if (victimRb != null)
    {
        victimRb.velocity = Vector3.zero;
        victimRb.angularVelocity = Vector3.zero;
        victimRb.isKinematic = true;
    }

    foreach (Collider col in victim.GetComponentsInChildren<Collider>())
        col.enabled = false;

    victim.position = new Vector3(victim.position.x, -1000f, victim.position.z);

    FaceTarget(victim);

    Debug.Log("[DragonAI] 🪂 LANDING...");
    SetOnly(hLands);
    yield return new WaitForSeconds(1.2f);
    if (isDead) yield break; // ✅ FIX

    Debug.Log($"[DragonAI] 🦷 BITING [{victim.name}]!");
    SetOnly(hBite);
    yield return new WaitForSeconds(1.4f);
    if (isDead) yield break; // ✅ FIX

    if (victim != null)
    {
        int remaining = GameObject.FindGameObjectsWithTag("NPC").Length - 1;
        Debug.Log($"[DragonAI] 💀 KILLED [{victim.name}]. NPCs remaining: {remaining}");
        Destroy(victim.gameObject);
    }

    if (isDead) yield break; // ✅ FIX

    Debug.Log("[DragonAI] 🛫 TAKING OFF — hunting next target...");
    SetOnly(hTakeOff);
    yield return new WaitForSeconds(1.0f);
    if (isDead) yield break; // ✅ FIX

    agent.isStopped = false;
    SetOnly(hFlyingFWD);
    isAttacking = false;
}

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;
        isAttacking = false; // ✅ FIX: cancel any active attack so coroutine can't resume
        npcTarget = null;    // ✅ FIX: clear target immediately
        agent.isStopped = true;  // ✅ FIX: stop movement immediately
        agent.ResetPath();       // ✅ FIX: clear path immediately
        StopAllCoroutines();
        StartCoroutine(DieDragon());
    }

    IEnumerator DieDragon()
    {
        isDead = true;
        Debug.Log("[DragonAI] ☠️ DRAGON IS DYING...");

        agent.isStopped = true;
        agent.ResetPath();
        agent.enabled = false;

        SetAllFalse();
        anim.SetBool(hDie, true);

        yield return new WaitForSeconds(disappearDelay);
        Debug.Log("[DragonAI] 💨 Dragon has vanished.");
        Destroy(gameObject);
    }

    void DragonWins()
    {
        if (isAttacking) return;
        Log("🏆 DRAGON WINS — all NPCs eliminated!");
        agent.isStopped = true;
        agent.ResetPath();
        SetOnly(hIdleAgressive);
    }

    void Log(string msg)
    {
        if (msg == lastLog) return;
        lastLog = msg;
        Debug.Log(msg);
    }

    float FlatDist(Vector3 a, Vector3 b) =>
        Vector3.Distance(new Vector3(a.x, 0, a.z), new Vector3(b.x, 0, b.z));

    void FaceTarget(Transform t)
    {
        if (t == null) return;
        Vector3 dir = (t.position - transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void SetOnly(int hash) { SetAllFalse(); anim.SetBool(hash, true); }

    void SetAllFalse()
    {
        anim.SetBool(hIdleSimple, false);
        anim.SetBool(hIdleAgressive, false);
        anim.SetBool(hIdleRestless, false);
        anim.SetBool(hWalk, false);
        anim.SetBool(hBattleStance, false);
        anim.SetBool(hBite, false);
        anim.SetBool(hDrakaris, false);
        anim.SetBool(hFlyingFWD, false);
        anim.SetBool(hFlyingAttack, false);
        anim.SetBool(hHover, false);
        anim.SetBool(hLands, false);
        anim.SetBool(hTakeOff, false);
        anim.SetBool(hDie, false);
    }
}