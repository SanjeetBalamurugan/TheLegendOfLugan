using System.Collections;
using UnityEngine;

public class TPVPlayerMove : MonoBehaviour
{
    [Header("Player Movement")]
    public CharacterController controller;
    public float moveSpeed = 5f;
    public float runSpeed = 5f;
    public float jumpHeight = 2f;

    [Header("Player GroundCheck")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    private float gravity = -9.81f;

    [Header("The Character Animator")]
    [SerializeField]
    private Animator animator;

    [Header("Animator Setup")]
    [SerializeField]
    private float smoothing = 0.03f; 
    [SerializeField]
    private float runningSmoothing = 0.03f; 

    private float targetX = 0f;
    private float targetY = 0f;

    private float currentX = 0f;
    private float currentY = 0f;

    private bool shiftHeldBeforeMovement = false;

    void Update()
    {
        targetX = 0f;
        targetY = 0f;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSmoothing = isRunning ? runningSmoothing : smoothing;

        ApplyGravity();
        HandleMovement(isRunning, currentSmoothing);

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

    private void HandleMovement(bool isRunning, float currentSmoothing)
    {
        if (Input.GetKey(KeyCode.W))
        {
            animator.Play("Player_WalkForward");
        }
        else if (Input.GetKey(KeyCode.S))
        {
            targetY = isRunning ? -1f : -0.5f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            targetX = isRunning ? -1f : -0.5f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            targetX = isRunning ? 1f : 0.5f;
        }

        Debug.Log(currentX);

        shiftHeldBeforeMovement = isRunning && Input.GetKey(KeyCode.W);
    }

    public bool IsGrounded() => isGrounded;
    public float GetGravity() => gravity;
}
