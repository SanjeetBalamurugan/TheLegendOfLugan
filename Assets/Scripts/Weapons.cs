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
    private Weapon weapon;
    private List<string> attackSequence = new List<string>();
    private float comboTimeLimit = 1f;
    private float comboTimer;

    public void Initialize(Weapon weapon, Animator animator)
    {
        this.weapon = weapon;
        this.animator = animator;
    }

    public override void Attack()
    {
        animator.SetTrigger(weapon.attackAnimationTrigger);
        attackSequence.Add("Attack");
        comboTimer = comboTimeLimit;
    }

    public override void StartCombo()
    {
        attackSequence.Clear();
        comboTimer = 0;
    }

    public override void HandleCombo(string input)
    {
        if (comboTimer > 0) comboTimer -= Time.deltaTime;

        if (comboTimer <= 0)
        {
            attackSequence.Clear();
        }

        if (input == "Light")
        {
            attackSequence.Add("Light");
        }
        else if (input == "Heavy" && attackSequence.Count >= 2 && attackSequence[0] == "Light" && attackSequence[1] == "Light")
        {
            animator.SetTrigger("HeavyCombo");
            attackSequence.Clear();
        }
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

