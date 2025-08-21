using System.Collections;
using UnityEngine;

public interface IPlayerState
{
    void Enter(PlayerStateManager player);
    void Exit();
    void Update();
    void HandleInput();
}

public class IdleState : IPlayerState
{
    private PlayerStateManager player;

    public void Enter(PlayerStateManager player)
    {
        this.player = player;
        player.GetAnimator().SetBool("IsMoving", false);
        Debug.Log("Idle");
    }

    public void Exit() {}

    public void Update() {
        HandleInput();
    }

    public void HandleInput()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Debug.Log("MoveStateChange");
            player.SetState(new MoveState());
        }
        if (Input.GetButtonDown("Fire1"))
        {
            player.SetState(new CombatState());
        }
    }
}

public class MoveState : IPlayerState
{
    private PlayerStateManager player;
    private float targetX = 0f;
    private float targetY = 0f;
    private float currentX = 0f;
    private float currentY = 0f;
    private float smoothing = 0.03f;
    private float runningSmoothing = 0.03f;
    private TPVPlayerMove _PlayerMove;

    public void Enter(PlayerStateManager player)
    {
        this.player = player;
        player.animator.SetBool("IsMoving", true);
        _PlayerMove = player.GetPlayerMove();
    }

    public void Exit()
    {
        player.animator.SetBool("IsMoving", false);
    }

    public void Update()
    {
        HandleMovementInput();
        UpdateAnimator();
    }

    public void HandleInput()
    {
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            player.SetState(new IdleState());
        }
        if (Input.GetButtonDown("Fire1"))
        {
            player.SetState(new CombatState());
        }
    }

    private void HandleMovementInput()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSmoothing = isRunning ? runningSmoothing : smoothing;

        targetX = 0f;
        targetY = 0f;

        if (Input.GetKey(KeyCode.W)) targetY = isRunning ? 1f : 0.5f;
        else if (Input.GetKey(KeyCode.S)) targetY = isRunning ? -1f : -0.5f;

        if (Input.GetKey(KeyCode.A)) targetX = isRunning ? -1f : -0.5f;
        else if (Input.GetKey(KeyCode.D)) targetX = isRunning ? 1f : 0.5f;

        currentX = Mathf.Lerp(currentX, targetX, currentSmoothing);
        currentY = Mathf.Lerp(currentY, targetY, currentSmoothing);

        Vector3 forward = _PlayerMove.gameObject.transform.forward;
        Vector3 right = _PlayerMove.gameObject.transform.right;
        float jUP = 0;

        if (Input.GetButtonDown("Jump") && _PlayerMove.IsGrounded())
        {
            jUP = Mathf.Sqrt(_PlayerMove.jumpHeight * -2f * _PlayerMove.GetGravity());
        }

        if (isRunning)
        {
            Vector3 moveDirection = forward * currentY + right * currentX + jUP * _PlayerMove.gameObject.transform.up;

            if (currentY > 0)
            {
                _PlayerMove.controller.Move(moveDirection * _PlayerMove.runSpeed * Time.deltaTime);
                _PlayerMove.gameObject.transform.rotation = Quaternion.LookRotation(forward);
            }
            else
            {
                _PlayerMove.controller.Move(moveDirection * _PlayerMove.runSpeed * Time.deltaTime);
            }
        }
        else
        {
            Vector3 moveDirection = forward * currentY + right * currentX;
            _PlayerMove.controller.Move(moveDirection * _PlayerMove.moveSpeed * Time.deltaTime);
        }
    }

    private void UpdateAnimator()
    {
        player.animator.SetFloat("x", currentX);
        player.animator.SetFloat("y", currentY);
    }
}



public class CombatState : IPlayerState
{
    private PlayerStateManager player;
    private bool isAttacking;

    public void Enter(PlayerStateManager player)
    {
        this.player = player;
        player.GetAnimator().SetTrigger("Attack");
        player.GetCombatSystem().StartCombo();
        isAttacking = true;
    }

    public void Exit()
    {
        player.GetAnimator().ResetTrigger("Attack");
        isAttacking = false;
    }

    public void Update() {}

    public void HandleInput()
    {
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            player.GetCombatSystem().LightAttack();
            isAttacking = true;
        }
        else if (Input.GetButtonDown("Fire2") && !isAttacking)
        {
            player.GetCombatSystem().HeavyAttack();
            isAttacking = true;
        }

        if (player.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("AttackFinish") && !isAttacking)
        {
            player.SetState(new IdleState());
        }
    }
}
