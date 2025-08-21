public class CombatState : IPlayerState
{
    private PlayerStateManager player;
    private ClaymoreWeapon claymoreWeapon;

    public void Enter(PlayerStateManager player)
    {
        this.player = player;
        claymoreWeapon = player.GetComponentInChildren<ClaymoreWeapon>();

        claymoreWeapon.ResetCombo();
        claymoreWeapon.CancelCombo();
    }

    public void Exit()
    {
        claymoreWeapon.ResetCombo();
    }

    public void Update()
    {
        HandleInput();
        claymoreWeapon.FinishCombo();
    }

    public void HandleInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            claymoreWeapon.TryCombo();
        }

        if (player.GetAnimator().GetCurrentAnimatorStateInfo(0).IsName("AttackFinish"))
        {
            player.SetState(new IdleState());
        }
    }
}
