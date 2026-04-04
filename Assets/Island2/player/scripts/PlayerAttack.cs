using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject arrowPrefab;
    public int maxHits = 5;
    private int currentHits = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (currentHits >= maxHits)
        {
            Debug.Log("Dragon defeated!");
            return;
        }

        Debug.Log("Shooting arrow");

        // Spawn arrow in front of player
        Instantiate(
            arrowPrefab,
            transform.position + transform.forward * 2f + Vector3.up * 1.5f,
            transform.rotation
        );
    }

    public void IncreaseHit()
    {
        currentHits++;
        Debug.Log("Hits: " + currentHits);
    }
}
