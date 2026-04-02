using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragonHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth    = 100f;
    public float damagePerHit = 5f;       // 20 arrows = dead

    [Header("Health Bar UI")]
    public Canvas    healthCanvas;         // World Space Canvas (child of dragon)
    public Image     healthBarFill;        // Red fill image
    public Image     healthBarBackground;  // Dark background image

    [Header("Bar Settings")]
    public float lerpSpeed    = 5f;        // Smoothness of bar shrink
    public Color fullColor    = Color.green;
    public Color halfColor    = Color.yellow;
    public Color lowColor     = Color.red;

    private float currentHealth;
    private float targetFill;             // what the bar is lerping toward
    private Camera mainCam;
    private DragonAI dragonAI;            // reference to notify AI of death

    void Start()
    {
        currentHealth = maxHealth;
        targetFill    = 1f;
        mainCam       = Camera.main;
        dragonAI      = GetComponent<DragonAI>();

        // Make sure bar starts full
        if (healthBarFill != null)
            healthBarFill.fillAmount = 1f;

        UpdateBarColor(1f);

        Debug.Log($"[DragonHealth] Initialized. HP: {currentHealth}/{maxHealth}. Needs {Mathf.RoundToInt(maxHealth / damagePerHit)} arrow hits to die.");
    }

    void Update()
    {
        // Billboard — always face the camera
        if (healthCanvas != null && mainCam != null)
            healthCanvas.transform.rotation = Quaternion.LookRotation(
                healthCanvas.transform.position - mainCam.transform.position);

        // Smoothly lerp the bar fill
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = Mathf.Lerp(
                healthBarFill.fillAmount,
                targetFill,
                Time.deltaTime * lerpSpeed);
        }
    }

    // ─── CALLED BY ARROW ──────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow"))
        {
            TakeDamage(damagePerHit);
            Destroy(other.gameObject);
        }
    }

    // ─── PUBLIC — can be called from anywhere ─────────────────
    public void TakeDamage(float dmg)
    {
        if (currentHealth <= 0f) return;

        currentHealth -= dmg;
        currentHealth  = Mathf.Clamp(currentHealth, 0f, maxHealth);

        float pct      = currentHealth / maxHealth;
        targetFill     = pct;

        UpdateBarColor(pct);

        int hitsLeft = Mathf.CeilToInt(currentHealth / damagePerHit);
        Debug.Log($"[DragonHealth] 🏹 Arrow hit! HP: {currentHealth}/{maxHealth} — {hitsLeft} hits left to kill.");

        if (currentHealth <= 0f)
            StartCoroutine(Die());
    }

    // ─── COLOR BASED ON HP ────────────────────────────────────
    void UpdateBarColor(float pct)
    {
        if (healthBarFill == null) return;

        if (pct > 0.5f)
            healthBarFill.color = Color.Lerp(halfColor, fullColor, (pct - 0.5f) * 2f);
        else
            healthBarFill.color = Color.Lerp(lowColor, halfColor, pct * 2f);
    }

    // ─── DEATH ────────────────────────────────────────────────
    IEnumerator Die()
    {
        Debug.Log("[DragonHealth] ☠️ Dragon HP reached 0 — dying!");

        // Hide health bar
        if (healthCanvas != null)
            healthCanvas.gameObject.SetActive(false);

        // Tell DragonAI to play death animation
        if (dragonAI != null)
            dragonAI.TriggerDeath();
        else
        {
            // Fallback if no DragonAI found
            Debug.LogWarning("[DragonHealth] DragonAI not found — destroying dragon directly.");
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }

        yield return null;
    }

    

    // ─── OPTIONAL: heal (for future use) ─────────────────────
    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        targetFill    = currentHealth / maxHealth;
        UpdateBarColor(targetFill);
        Debug.Log($"[DragonHealth] 💚 Healed {amount}. HP: {currentHealth}/{maxHealth}");
    }
}