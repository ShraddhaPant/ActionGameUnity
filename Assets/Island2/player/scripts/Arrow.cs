using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public float maxDistance = 50f;
    public float arcHeight = 0.3f;

    private Rigidbody rb;
    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;

        GameObject dragon = GameObject.FindGameObjectWithTag("Dragon");

        if (rb != null)
        {
            if (dragon != null)
            {
                // 🎯 Target dragon
                Vector3 targetPos = dragon.transform.position + Vector3.up * 1.5f;

                Vector3 direction = (targetPos - transform.position).normalized;

                // 🔥 Add arc
                direction += Vector3.up * arcHeight;

                rb.velocity = direction.normalized * speed;
            }
            else
            {
                // fallback
                rb.velocity = transform.forward * speed;
            }
        }

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Destroy if too far
        if (Vector3.Distance(startPos, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dragon"))
        {
            Debug.Log("🔥 Hit Dragon!");

            DragonHealth dh = other.GetComponent<DragonHealth>();
            if (dh != null)
            {
                dh.TakeDamage(damage);
            }

            PlayerAttack player = FindObjectOfType<PlayerAttack>();
            if (player != null)
            {
                player.IncreaseHit();
            }

            Destroy(gameObject);
        }
    }
}