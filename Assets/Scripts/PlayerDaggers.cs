using UnityEngine;

public class PlayerDaggers : MonoBehaviour
{
    public float attackRange = 2f;
    public float takedownRange = 2f;
    public LayerMask enemyLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Takedown();
        }
    }

    void Attack()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange, enemyLayer))
        {
            Debug.Log("Hit Enemy");
            Destroy(hit.collider.gameObject);
        }
    }

    void Takedown()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, takedownRange, enemyLayer))
        {
            Transform enemy = hit.collider.transform;

            Vector3 directionToPlayer = (transform.position - enemy.position).normalized;
            float dot = Vector3.Dot(enemy.forward, directionToPlayer);

            if (dot > 0.5f)
            {
                Debug.Log("Silent Takedown SUCCESS");
                Destroy(enemy.gameObject);
            }
            else
            {
                Debug.Log("Not behind enemy");
            }
        }
    }
}