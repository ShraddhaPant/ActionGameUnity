using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject arrowPrefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Debug.Log("Shooting arrow");

        Instantiate(
            arrowPrefab,
            transform.position + transform.forward * 2f + Vector3.up * 1.5f,
            transform.rotation
        );
    }

    // ✅ Kept — Arrow.cs calls this
    public void IncreaseHit()
    {
        Debug.Log("Hits: " + 1);
    }
}