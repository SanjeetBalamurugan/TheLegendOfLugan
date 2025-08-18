using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public Animator animator;
    public float lightAttackCooldown = 0.5f;
    public float heavyAttackCooldown = 1f;

    private bool canLightAttack = true;
    private bool canHeavyAttack = true;

    void Start()
    {
        animator = GetComponent<Animator>();
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
