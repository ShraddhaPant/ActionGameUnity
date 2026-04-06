using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragonHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth    = 100f;
    public float damagePerHit = 5f;

    [Header("Health Bar UI")]
    public Canvas    healthCanvas;
    public Image     healthBarFill;
    public Image     healthBarBackground;

    [Header("Bar Settings")]
    public float lerpSpeed    = 5f;
    public Color fullColor    = Color.green;
    public Color halfColor    = Color.yellow;
    public Color lowColor     = Color.red;

    private float currentHealth;
    private float targetFill;
    private Camera mainCam;
    private DragonAI dragonAI;

    void Start()
    {
        currentHealth = maxHealth;
        targetFill    = 1f;
        mainCam       = Camera.main;
        dragonAI      = GetComponent<DragonAI>();

        if (healthBarFill != null)
            healthBarFill.fillAmount = 1f;

        UpdateBarColor(1f);

        Debug.Log($"[DragonHealth] Initialized. HP: {currentHealth}/{maxHealth}. Needs {Mathf.RoundToInt(maxHealth / damagePerHit)} arrow hits to die.");
    }

    void Update()
    {
        if (healthCanvas != null && mainCam != null)
            healthCanvas.transform.rotation = Quaternion.LookRotation(
                healthCanvas.transform.position - mainCam.transform.position);

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = Mathf.Lerp(
                healthBarFill.fillAmount,
                targetFill,
                Time.deltaTime * lerpSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow"))
        {
            TakeDamage(damagePerHit);
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(float dmg)
    {
        if (currentHealth <= 0f) return;

        currentHealth -= dmg;
        currentHealth  = Mathf.Clamp(currentHealth, 0f, maxHealth);

        float pct  = currentHealth / maxHealth;
        targetFill = pct;

        UpdateBarColor(pct);

        int hitsLeft = Mathf.CeilToInt(currentHealth / damagePerHit);
        Debug.Log($"[DragonHealth] 🏹 Arrow hit! HP: {currentHealth}/{maxHealth} — {hitsLeft} hits left to kill.");

        if (currentHealth <= 0f)
            StartCoroutine(Die());
    }

    void UpdateBarColor(float pct)
    {
        if (healthBarFill == null) return;

        if (pct > 0.5f)
            healthBarFill.color = Color.Lerp(halfColor, fullColor, (pct - 0.5f) * 2f);
        else
            healthBarFill.color = Color.Lerp(lowColor, halfColor, pct * 2f);
    }

    // ─── ONLY THIS CHANGED ────────────────────────────────────
    IEnumerator Die()
    {
        Debug.Log("[DragonHealth] ☠️ Dragon HP reached 0 — dying!");

        // ✅ Stop this script immediately
        this.enabled = false;

        // ✅ Hide health bar
        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(false);

        // ✅ Tell DragonAI to die — this also calls StopAllCoroutines inside
        if (dragonAI != null)
            dragonAI.TriggerDeath();
        else
            Debug.LogWarning("[DragonHealth] DragonAI not found!");

        // ✅ GUARANTEED destroy — fires no matter what after timer
        float waitTime = (dragonAI != null) ? dragonAI.disappearDelay + 2f : 5f;
        yield return new WaitForSeconds(waitTime);

        if (gameObject != null)
        {
            Debug.Log("[DragonHealth] 🗑️ Guaranteed destroy fired.");
            Destroy(gameObject);
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        targetFill    = currentHealth / maxHealth;
        UpdateBarColor(targetFill);
        Debug.Log($"[DragonHealth] 💚 Healed {amount}. HP: {currentHealth}/{maxHealth}");
    }
}