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

    public void Update() {}

    public void HandleInput()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
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

    public void Enter(PlayerStateManager player)
    {
        this.player = player;
        player.GetAnimator().SetBool("IsMoving", true);
    }

    public void Exit()
    {
        player.GetAnimator().SetBool("IsMoving", false);
    }

    public void Update()
    {
        player.GetPlayerMove().UpdateMovement();
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
}

public class CombatState : IPlayerState
{
    private PlayerStateManager player;

    public void Enter(PlayerStateManager player)
    {
        this.player = player;
        player.GetAnimator().SetTrigger("Attack");
        player.GetCombatSystem().StartCombo();
    }

    public void Exit() {}

    public void Update() {}

    public void HandleInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            player.GetCombatSystem().LightAttack();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            player.GetCombatSystem().HeavyAttack();
        }

        if (player.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("AttackFinish"))
        {
            player.SetState(new IdleState());
        }
    }
}

