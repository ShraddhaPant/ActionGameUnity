using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject winText;
    public ZombieSpawner spawner;

    void Update()
{
    int zombiesLeft = GameObject.FindGameObjectsWithTag("Zombie").Length;

    // 🔥 Only check win AFTER all zombies are spawned
    if (spawner != null &&
        zombiesLeft == 0 &&
        spawner.enabled == false)
    {
        WinGame();
    }
}

    void WinGame()
    {
        Debug.Log("🏆 YOU WIN!");

        if (winText != null)
            winText.SetActive(true);

        Time.timeScale = 0f;
        enabled = false;
    }
}
