using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 10;
    public float maxDistance = 10f;

    private Rigidbody rb;
    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;

        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Destroy arrow if it goes too far
        if (Vector3.Distance(startPos, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dragon"))
        {
            Debug.Log("Hit Dragon");

            other.GetComponent<DragonHealth>()?.TakeDamage(damage);

            PlayerAttack player = FindObjectOfType<PlayerAttack>();
            if (player != null)
            {
                player.IncreaseHit();
            }

            Destroy(gameObject);
        }
    }
}