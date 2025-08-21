using UnityEngine;
using System.Collections.Generic;

public abstract class BaseWeapon : MonoBehaviour
{
    public abstract void Attack();
    public abstract void StartCombo();
    public abstract void HandleCombo(string input);
}

public class ClaymoreWeapon : BaseWeapon
{
    private Animator animator;
    private int comboCount = 0;
    private bool isAttacking = false;
    private float comboTimer = 0f;
    private float comboTimeLimit = 0.5f;
    private float attackCooldown = 0.1f;
    private float attackCooldownTimer = 0f;
    private bool canChainCombo = true;

    private string[] comboAnimations = new string[]
    {
        "ClaymoreAttack_Combo1",
        "ClaymoreAttack_Combo2",
        "ClaymoreAttack_Combo3",
        "ClaymoreAttack_Combo4"
    };

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger(comboAnimations[comboCount]);
            comboTimer = comboTimeLimit;
            attackCooldownTimer = attackCooldown;
            StartCoroutine(ComboCooldown());
        }
    }

    public override void StartCombo()
    {
        comboCount = 0;
        isAttacking = false;
    }

    public override void HandleCombo(string input)
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            ResetCombo();
        }

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (input == "Fire1" && canChainCombo)
        {
            TryCombo();
        }
    }

    private void TryCombo()
    {
        if (isAttacking || !canChainCombo) return;

        if (comboCount < 4)
        {
            comboCount++;
            string attackTrigger = comboAnimations[comboCount - 1];
            animator.SetTrigger(attackTrigger);
            comboTimer = comboTimeLimit;
            attackCooldownTimer = attackCooldown;
            StartCoroutine(ComboCooldown());
        }
        else
        {
            ResetCombo();
        }
    }

    private IEnumerator ComboCooldown()
    {
        canChainCombo = false;
        yield return new WaitForSeconds(attackCooldown);
        canChainCombo = true;
    }

    private void ResetCombo()
    {
        comboCount = 0;
        isAttacking = false;
        canChainCombo = true;
    }
}


public class BowWeapon : BaseWeapon
{
    private Animator animator;
    private Weapon weapon;

    public void Initialize(Weapon weapon, Animator animator)
    {
        this.weapon = weapon;
        this.animator = animator;
    }

    public override void Attack()
    {
        animator.SetTrigger(weapon.attackAnimationTrigger);
    }

    public override void StartCombo()
    {
    }

    public override void HandleCombo(string input)
    {
        if (input == "Fire1")
        {
            animator.SetTrigger("BowRapidFire");
        }
    }
}

