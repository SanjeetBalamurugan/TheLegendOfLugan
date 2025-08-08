using UnityEngine;

public class TPVCamera : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // Assign the main camera
    private CharacterController controller;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Camera Orbit")]
    public float mouseSensitivity = 200f;
    public float minPitch = -30f;
    public float maxPitch = 60f;
    private float yaw;   // Horizontal rotation around player
    private float pitch; // Vertical camera tilt
    public float cameraDistance = 4f;
    public Vector3 cameraOffset = new Vector3(0, 2, 0); // Camera height offset

    [Header("Gravity & Jumping")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 angles = cameraTransform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        HandleCameraRotation();
        HandleMovement();
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPosition = transform.position + cameraOffset - rotation * Vector3.forward * cameraDistance;

        cameraTransform.position = targetPosition;
        cameraTransform.rotation = rotation;
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            // Calculate movement direction relative to camera
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + yaw;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // Jump
        if (controller.isGrounded)
        {
            velocity.y = -2f; // Keeps grounded
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}
