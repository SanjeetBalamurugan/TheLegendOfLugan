using System.Collections;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public abstract void Attack();
    public abstract void StartCombo();
    public abstract void HandleCombo(string input);
}

public class ClaymoreWeapon : BaseWeapon
{
    private Animator animator;
    private int comboIndex = 0;
    private bool isAttacking = false;
    private float comboTimer = 0f;
    private float comboTimeLimit = 0.5f;
    private bool canChain = true;
    private float chainCooldown = 0.1f;

    [SerializeField] private string[] comboAnimations = new string[]
    {
        "ClaymoreAttack_Combo1",
        "ClaymoreAttack_Combo2",
        "ClaymoreAttack_Combo3",
        "ClaymoreAttack_Combo4"
    };

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f) ResetCombo();
        }
    }

    public override void Attack()
    {
        if (isAttacking || !canChain) return;
        PlayNext();
    }

    public override void StartCombo()
    {
        comboIndex = 0;
        isAttacking = false;
        canChain = true;
        comboTimer = 0f;
    }

    public override void HandleCombo(string input)
    {
        if (input != "Fire1") return;
        if (!isAttacking && canChain)
        {
            PlayNext();
        }
    }

    private void PlayNext()
    {
        if (comboIndex >= comboAnimations.Length)
        {
            ResetCombo();
            return;
        }
        animator.SetTrigger(comboAnimations[comboIndex]);
        comboIndex++;
        isAttacking = true;
        comboTimer = comboTimeLimit;
        StartCoroutine(ChainWindow());
    }

    private IEnumerator ChainWindow()
    {
        canChain = false;
        yield return new WaitForSeconds(chainCooldown);
        canChain = true;
        isAttacking = false;
    }

    private void ResetCombo()
    {
        comboIndex = 0;
        isAttacking = false;
        canChain = true;
        comboTimer = 0f;
    }
}

public class BowWeapon : BaseWeapon
{
    private Animator animator;
    [SerializeField] private string attackTrigger = "BowAttack";
    [SerializeField] private string rapidTrigger = "BowRapidFire";
    [SerializeField] private float fireCooldown = 0.15f;
    private float cd;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (cd > 0f) cd -= Time.deltaTime;
    }

    public void Initialize(Animator anim)
    {
        animator = anim;
    }

    public override void Attack()
    {
        if (cd > 0f) return;
        animator.SetTrigger(attackTrigger);
        cd = fireCooldown;
    }

    public override void StartCombo() {}

    public override void HandleCombo(string input)
    {
        if (input == "Fire1" && cd <= 0f)
        {
            animator.SetTrigger(rapidTrigger);
            cd = fireCooldown;
        }
    }
}
