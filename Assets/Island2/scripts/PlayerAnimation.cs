using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float aimSpeed = 1.5f;
    public float rotationSpeed = 10f;

    [Header("References")]
    public Transform firePoint;
    public GameObject arrowPrefab;

    private Rigidbody rb;
    private Animator animator;

    private Vector3 movement;
    private float currentSpeed;
    private bool isAiming;
    

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        // 🎮 INPUT
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        movement = new Vector3(moveX, 0f, moveZ);

        // 🏹 AIM (Right Click)
        isAiming = Input.GetMouseButton(1);
        animator.SetBool("isAiming", isAiming);

        // 🎯 SHOOT (Left Click)
        if (Input.GetMouseButtonDown(0) && isAiming)
        {
            animator.SetTrigger("Shoot");
        }
        if (Input.GetKeyDown(KeyCode.P))
    {
        animator.Play("Aiming");
    }

        // 🚶 ANIMATION (Blend Tree)
        float speed = movement.magnitude;
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

        // ⚡ SPEED CONTROL
        if (isAiming)
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

            // 🚶 MOVE
            rb.MovePosition(rb.position + moveDir * currentSpeed * Time.fixedDeltaTime);

            // 🔄 ROTATE
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    // 🏹 CALLED BY ANIMATION EVENT (BEST METHOD)
    public void ShootArrow()
    {
        Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
    }
}