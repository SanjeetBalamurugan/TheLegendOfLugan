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
    }

    public void Exit() {}

    public void Update()
    {
        HandleInput();
    }

    public void HandleInput()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            player.SetState(new MoveState());
        }
        else if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
        {
            player.SetState(new CombatState());
        }
    }
}

public class MoveState : IPlayerState
{
    private PlayerStateManager player;
    private TPVPlayerMove mover;
    private float targetX;
    private float targetY;
    private float currentX;
    private float currentY;
    private float smoothing = 0.08f;
    private float runningSmoothing = 0.08f;
    private float verticalVelocity;

    public void Enter(PlayerStateManager player)
    {
        this.player = player;
        mover = player.GetPlayerMove();
        player.GetAnimator().SetBool("IsMoving", true);
        verticalVelocity = 0f;
    }

    public void Exit()
    {
        player.GetAnimator().SetBool("IsMoving", false);
        player.GetAnimator().SetFloat("x", 0f);
        player.GetAnimator().SetFloat("y", 0f);
    }

    public void Update()
    {
        HandleMovement();
        HandleInput();
        UpdateAnimator();
    }

    public void HandleInput()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
        {
            player.SetState(new CombatState());
            return;
        }

        if (Mathf.Approximately(Input.GetAxisRaw("Horizontal"), 0f) && Mathf.Approximately(Input.GetAxisRaw("Vertical"), 0f) && mover.IsGrounded() && Mathf.Abs(verticalVelocity) < 0.01f)
        {
            player.SetState(new IdleState());
        }
    }

    private void HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float lerp = isRunning ? runningSmoothing : smoothing;

        targetX = 0f;
        targetY = 0f;

        if (Input.GetKey(KeyCode.W)) targetY = isRunning ? 1f : 0.5f;
        else if (Input.GetKey(KeyCode.S)) targetY = isRunning ? -1f : -0.5f;

        if (Input.GetKey(KeyCode.A)) targetX = isRunning ? -1f : -0.5f;
        else if (Input.GetKey(KeyCode.D)) targetX = isRunning ? 1f : 0.5f;

        currentX = Mathf.Lerp(currentX, targetX, lerp);
        currentY = Mathf.Lerp(currentY, targetY, lerp);

        if (mover.IsGrounded())
        {
            if (verticalVelocity < 0f) verticalVelocity = -2f;
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(mover.jumpHeight * -2f * mover.GetGravity());
            }
        }
        verticalVelocity += mover.GetGravity() * Time.deltaTime;

        Vector3 forward = mover.transform.forward;
        Vector3 right = mover.transform.right;
        Vector3 move = forward * currentY + right * currentX;
        if (isRunning) move *= mover.runSpeed;
        else move *= mover.moveSpeed;

        move += Vector3.up * verticalVelocity;
        mover.controller.Move(move * Time.deltaTime);

        if (isRunning && currentY > 0.01f)
        {
            mover.transform.rotation = Quaternion.LookRotation(forward);
        }
        else if (move.sqrMagnitude > 0.0001f)
        {
            Vector3 flat = new Vector3(move.x, 0f, move.z);
            if (flat.sqrMagnitude > 0.0001f)
            {
                mover.transform.rotation = Quaternion.LookRotation(flat);
            }
        }
    }

    private void UpdateAnimator()
    {
        player.GetAnimator().SetFloat("x", currentX);
        player.GetAnimator().SetFloat("y", currentY);
    }
}

public class CombatState : IPlayerState
{
    private PlayerStateManager player;
    private bool started;

    public void Enter(PlayerStateManager player)
    {
        this.player = player;
        started = false;
    }

    public void Exit() {}

    public void Update()
    {
        HandleInput();
        if (!started)
        {
            player.GetCombatSystem().StartCombo();
            started = true;
        }
    }

    public void HandleInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            player.GetCombatSystem().Attack("Fire1");
            player.GetCombatSystem().HandleComboInput("Fire1");
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            player.GetCombatSystem().Attack("Fire2");
            player.GetCombatSystem().HandleComboInput("Fire2");
        }

        if (!Input.GetButton("Fire1") && !Input.GetButton("Fire2"))
        {
            AnimatorStateInfo st = player.GetAnimator().GetCurrentAnimatorStateInfo(0);
            if (!st.loop && st.normalizedTime >= 1f)
            {
                player.SetState(new IdleState());
            }
        }
    }
}
