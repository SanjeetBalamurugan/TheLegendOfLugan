using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private Animator animator;
    private PlayerStateManager stateManager;
    public float lightAttackCooldown = 0.5f;
    public float heavyAttackCooldown = 1f;

    private bool canLightAttack = true;
    private bool canHeavyAttack = true;

    public void Start()
    {
        stateManager = GetComponent<PlayerStateManager>();
        animator = stateManager.GetAnimator();
    }

    public void StartCombo()
    {
        canLightAttack = true;
        canHeavyAttack = true;
    }

    public void LightAttack()
    {
        if (canLightAttack)
        {
            animator.SetTrigger("LightAttack");
            canLightAttack = false;
            Invoke(nameof(ResetLightAttack), lightAttackCooldown);
        }
    }

    public void HeavyAttack()
    {
        if (canHeavyAttack)
        {
            animator.SetTrigger("HeavyAttack");
            canHeavyAttack = false;
            Invoke(nameof(ResetHeavyAttack), heavyAttackCooldown);
        }
    }

    private void ResetLightAttack() => canLightAttack = true;
    private void ResetHeavyAttack() => canHeavyAttack = true;
}
