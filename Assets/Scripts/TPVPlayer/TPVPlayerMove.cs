using UnityEngine;

public class TPVPlayerMove : MonoBehaviour
{
    [Header("Player Movement")]
    public CharacterController controller;
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Camera playerCam;

    [Header("Player GroundCheck")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    private float gravity = -9.81f;

    [Header("Player Animation")]
    [SerializeField] private Animator animator;

    [Header("Player Aim")]
    [SerializeField] private Camera aimCam;
    private bool isAiming;

    private void Start()
    {
        aimCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        isAiming = Input.GetMouseButton(1);

        if(isAiming && !isRunning)
        {
            aimCam.gameObject.SetActive(true);
            playerCam.gameObject.SetActive(false);

            animator.SetLayerWeight(1, 1f);
        } else
        {
            aimCam.gameObject.SetActive(false);
            playerCam.gameObject.SetActive(true);

            animator.SetLayerWeight(1, 0f);
        }


        HandleMovement(isRunning);
        ApplyGravity();

        controller.Move(velocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private void HandleMovement(bool isRunning)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            float speed = isRunning ? runSpeed : moveSpeed;

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            if(isRunning)
            {
                animator.Play("StandardAnim_Running");
            } else {
                animator.Play("Walking");
            }
        } else
        {
            animator.Play("Player_Idle");
        }
    }

    public bool IsGrounded() => isGrounded;
    public float GetGravity() => gravity;
}
