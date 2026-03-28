using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform player;

    public int maxZombies = 3;
    public float spawnRadius = 35f;
    public float spawnInterval = 10f;

    public int totalToSpawn = 15; // 🔥 NEW
    private int spawnedCount = 0; // 🔥 NEW

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            int currentZombies = GameObject.FindGameObjectsWithTag("Zombie").Length;

            if (currentZombies < maxZombies && spawnedCount < totalToSpawn) // 🔥 FIX
            {
                SpawnZombie();
            }

            if (spawnedCount >= totalToSpawn)
            {
                enabled = false;
            }
        }
    }

    void SpawnZombie()
    {
        Vector3 randomPos = player.position + Random.insideUnitSphere * spawnRadius;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 20f, NavMesh.AllAreas))
        {
            Instantiate(zombiePrefab, hit.position, Quaternion.identity);
            spawnedCount++; // 🔥 IMPORTANT
        }
    }
}