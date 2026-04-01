using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float aimSpeed = 1.5f;
    public float rotationSpeed = 10f;

    public float gravity = -9.8f;
    public float groundCheckDistance = 0.2f;

    public Transform firePoint;
    public GameObject arrowPrefab;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 movement;
    private float currentSpeed;

    Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // INPUT
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        movement = new Vector3(moveX, 0f, moveZ).normalized;

        // AIM
        bool isAiming = Input.GetMouseButton(1);
        animator.SetBool("isAiming", isAiming);

        // SHOOT
        if (Input.GetMouseButtonDown(0) && isAiming)
        {
            animator.SetTrigger("Shoot");
        }

        // SPEED CONTROL
        if (isAiming)
            currentSpeed = aimSpeed;
        else if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = runSpeed;
        else
            currentSpeed = walkSpeed;

        // MOVEMENT
        if (movement.magnitude > 0.1f)
        {
            Vector3 move = movement * currentSpeed;
            controller.Move(move * Time.deltaTime);

            // ROTATION
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // GRAVITY (IMPORTANT)
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // keeps player grounded
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ANIMATION
        animator.SetFloat("Speed", movement.magnitude, 0.1f, Time.deltaTime);
    }

    public void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * 20f;
    }
}