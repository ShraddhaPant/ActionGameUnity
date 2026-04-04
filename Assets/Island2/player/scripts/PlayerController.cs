using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 12f;
    public float runSpeed = 1.5f;
    public float aimSpeed = 7f;
    public float rotationSpeed = 13f;

    private Rigidbody rb;
    private Vector3 movement;
    private float currentSpeed;
    private bool isAiming;

    Animator animator; // ✅ REQUIRED
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>(); // ✅ REQUIRED
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        movement = new Vector3(moveX, 0f, moveZ);

        // AIM
        bool isAimingNow = Input.GetMouseButton(1);
        animator.SetBool("isAiming", isAimingNow);

        // SHOOT
        if (Input.GetMouseButtonDown(0) && isAimingNow)
        {
            animator.SetTrigger("Shoot");
        }

        // Movement animation
        float speed = movement.magnitude;
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

        // Speed control
        if (isAimingNow)
            currentSpeed = aimSpeed;
        else if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = runSpeed;
        else
            currentSpeed = walkSpeed;
    }

    void FixedUpdate()
{
    if (movement.magnitude > 0.1f)
    {
        Vector3 moveDir = movement.normalized;

        rb.MovePosition(rb.position + moveDir * currentSpeed * Time.fixedDeltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }
}
}