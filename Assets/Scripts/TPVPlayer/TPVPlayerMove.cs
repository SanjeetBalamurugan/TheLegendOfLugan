using UnityEngine;

public class TPVPlayerMove : MonoBehaviour
{
    public CharacterController controller;
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Camera mainCamera;
    public Transform groundCheck;
    public float groundDistance = 0.4f; 
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool isGrounded;
    private float gravity = -9.81f;
    public float jumpHeight = 2f;
    private bool isJumping;
    [SerializeField] private Animator animator;
    private static readonly int IdleAnim  = Animator.StringToHash("Player_Idle");
    private static readonly int WalkAnim  = Animator.StringToHash("Walking");
    private static readonly int RunAnim   = Animator.StringToHash("StandardAnim_Running");
    private static readonly int JumpAnim  = Animator.StringToHash("Jumping");
    private int currentAnimState;
    private bool isAiming;
    private const string HorizontalAxis = "Horizontal";
    private const string VerticalAxis = "Vertical";

    void Start()
    {
        currentAnimState = IdleAnim;
        animator.Play(currentAnimState);
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        float horizontal = Input.GetAxisRaw(HorizontalAxis);
        float vertical = Input.GetAxisRaw(VerticalAxis);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        isAiming = Input.GetMouseButton(1);
        HandleJump();
        ApplyGravity();
        if (!isJumping) HandleMovement(horizontal, vertical, isRunning);
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            ChangeAnimationState(JumpAnim);
        }
        else if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            isJumping = false;
            if (currentAnimState != IdleAnim) ChangeAnimationState(IdleAnim);
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded && !isJumping) velocity.y += gravity * Time.deltaTime;
    }

    private void HandleMovement(float horizontal, float vertical, bool isRunning)
    {
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            float speed = isRunning ? runSpeed : moveSpeed;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
            if (isRunning) ChangeAnimationState(RunAnim);
            else ChangeAnimationState(WalkAnim);
        }
        else if (isGrounded) ChangeAnimationState(IdleAnim);
    }

    private void ChangeAnimationState(int newState)
    {
        if (currentAnimState == newState) return;
        animator.CrossFade(newState, 0.1f);
        currentAnimState = newState;
    }

    public bool GetAimValue() => isAiming;
    public bool IsGrounded() => isGrounded;
    public float GetGravity() => gravity;
}
