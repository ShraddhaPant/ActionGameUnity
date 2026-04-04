using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CitizenSpawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject citizenPrefab;
    public int numberOfCitizens = 10;
    public float spawnRadius = 50f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float raycastHeight = 10f;

    void Start()
    {
        SpawnCitizens();
    }

    void SpawnCitizens()
    {
        if (citizenPrefab == null)
        {
            Debug.LogError("CitizenSpawner: No prefab assigned!");
            return;
        }

        int spawned = 0;
        int attempts = 0;

        while (spawned < numberOfCitizens && attempts < numberOfCitizens * 5)
        {
            attempts++;

            // Random point within radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = new Vector3(
                transform.position.x + randomCircle.x,
                transform.position.y + raycastHeight,
                transform.position.z + randomCircle.y
            );

            // Raycast down to find ground
            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, raycastHeight * 2, groundLayer))
            {
                GameObject citizen = Instantiate(citizenPrefab, hit.point, Quaternion.identity);
                citizen.name = "Citizen_" + spawned;
                spawned++;
            }
        }

        Debug.Log($"CitizenSpawner: Spawned {spawned} citizens.");
    }
}