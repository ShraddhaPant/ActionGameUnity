using UnityEngine;

public class PlayerStealthController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float crouchSpeed = 2f;

    public float visibility = 1f;

    public Transform cameraTransform;   // Assign Main Camera here
    public float rotationSpeed = 10f;

    private CharacterController controller;
    private Renderer rend;

    private Color currentColor;
    private Color targetColor;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        rend = GetComponent<Renderer>();

        currentColor = rend.material.color;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(h, 0, v);

        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        float speed = isCrouching ? crouchSpeed : walkSpeed;

        // Visibility
        visibility = isCrouching ? 0.3f : 1f;

        // 🎨 Smooth color transition
        targetColor = isCrouching ? Color.gray : Color.white;
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * 5f);
        rend.material.color = currentColor;

        // 🎯 CAMERA RELATIVE MOVEMENT (FINAL FIX)
        if (inputDirection.magnitude >= 0.1f)
        {
            // Get camera directions
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // Ignore vertical tilt
            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            // Calculate move direction
            Vector3 moveDir = forward * v + right * h;

            // Smooth rotation towards movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move player
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
}