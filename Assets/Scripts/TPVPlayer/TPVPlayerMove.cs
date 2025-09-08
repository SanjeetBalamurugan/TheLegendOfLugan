using UnityEngine;

public class TPVPlayerMove : MonoBehaviour
{
    [Header("Player Movement")]
    public CharacterController controller;
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public GameObject playerCam;
    public Camera actualCam;

    [Header("Player GroundCheck")]
    public Transform groundCheck;
    public float groundDistance = 0.4f; 
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    private float gravity = -9.81f;

    [Header("Player Jumping")]
    public float jumpHeight = 2f;

    [Header("Player Animation")]
    [SerializeField] private Animator animator;

    private static readonly int IdleAnim  = Animator.StringToHash("Player_Idle");
    private static readonly int WalkAnim  = Animator.StringToHash("Walking");
    private static readonly int RunAnim   = Animator.StringToHash("StandardAnim_Running");
    private static readonly int JumpAnim  = Animator.StringToHash("Jumping");

    private int currentAnimState;

    [Header("Player Aim")]
    [SerializeField] private GameObject aimCam;
    private bool isAiming;
    private bool lastAimingState = false;

    [Header("Player Aim Rotation Settings")]
    [SerializeField] private Vector3 playerAimRotation; // rotation offset while aiming
    [SerializeField] private Transform playerModel;     // the visible character model
    [SerializeField] private float aimRotateSpeed = 5f; // how fast rotation interpolates

    private Quaternion defaultRotationOffset;
    private Quaternion aimRotationOffset;

    private const string HorizontalAxis = "Horizontal";
    private const string VerticalAxis = "Vertical";

    private void Start()
    {
        aimCam.SetActive(false);
        playerCam.SetActive(true);

        currentAnimState = IdleAnim;
        animator.Play(currentAnimState);

        // store default & aiming rotations
        defaultRotationOffset = Quaternion.identity;
        aimRotationOffset = Quaternion.Euler(playerAimRotation);
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        float horizontal = Input.GetAxisRaw(HorizontalAxis);
        float vertical = Input.GetAxisRaw(VerticalAxis);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        isAiming = Input.GetMouseButton(1);

        HandleAiming(isRunning);
        HandleMovement(horizontal, vertical, isRunning);
        HandleJump();
        ApplyGravity();

        controller.Move(velocity * Time.deltaTime);
    }

    // --- AIMING ---
    private void HandleAiming(bool isRunning)
    {
        if (isAiming != lastAimingState)
        {
            if (isAiming && !isRunning)
                OnAimStart();
            else
                OnAimEnd();

            lastAimingState = isAiming;
        }

        // Smooth model rotation while aiming / not aiming
        if (playerModel != null)
        {
            Quaternion targetRot = isAiming ? aimRotationOffset : defaultRotationOffset;
            playerModel.localRotation = Quaternion.Slerp(
                playerModel.localRotation,
                targetRot,
                Time.deltaTime * aimRotateSpeed
            );
        }
    }

    private void OnAimStart()
    {
        aimCam.SetActive(true);
        playerCam.SetActive(false);
        animator.SetLayerWeight(1, 1f);
    }

    private void OnAimEnd()
    {
        aimCam.SetActive(false);
        playerCam.SetActive(true);
        animator.SetLayerWeight(1, 0f);
    }

    // --- JUMPING ---
    private void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            ChangeAnimationState(JumpAnim);
        }
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        else
            velocity.y += gravity * Time.deltaTime;
    }

    // --- MOVEMENT ---
    private void HandleMovement(float horizontal, float vertical, bool isRunning)
    {
        if (isAiming)
            HandleAimMovement(horizontal, vertical);
        else
            HandleNormalMovement(horizontal, vertical, isRunning);
    }

    private void HandleNormalMovement(float horizontal, float vertical, bool isRunning)
    {
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg 
                                + actualCam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, 
                                                ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            float speed = isRunning ? runSpeed : moveSpeed;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            if (isRunning)
                ChangeAnimationState(RunAnim);
            else
                ChangeAnimationState(WalkAnim);
        }
        else if (isGrounded)
        {
            ChangeAnimationState(IdleAnim);
        }
    }

    private void HandleAimMovement(float horizontal, float vertical)
    {
        transform.rotation = Quaternion.Euler(0f, actualCam.transform.eulerAngles.y, 0f);

        Vector3 moveDir = (actualCam.transform.forward * vertical 
                         + actualCam.transform.right * horizontal).normalized;

        if (moveDir.magnitude >= 0.1f)
        {
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
            ChangeAnimationState(WalkAnim);
        }
        else if (isGrounded)
        {
            ChangeAnimationState(IdleAnim);
        }
    }

    // --- ANIMATION ---
    private void ChangeAnimationState(int newState)
    {
        if (currentAnimState == newState) return; 
        animator.CrossFade(newState, 0.15f); // smoother transition
        currentAnimState = newState;
    }

    // --- GETTERS ---
    public bool IsGrounded() => isGrounded;
    public float GetGravity() => gravity;
    public bool GetAimValue() => isAiming;
    public Camera GetActualCam() => actualCam;
}
