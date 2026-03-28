using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    private bool isDead = false;

    private ZombieController controller;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;

        controller = GetComponent<ZombieController>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        Debug.Log("🧟 Zombie Hit! Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return; // 🔥 prevents double death
        isDead = true;

        Debug.Log("💀 Zombie Died!");

        // Disable collider so it doesn't block shots
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Stop movement + play death animation
        if (controller != null)
        {
            controller.OnDeath();
        }

        // Optional: destroy after few seconds
        Destroy(gameObject, 5f);
    }
}