using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private Animator animator;
    private PlayerWeaponManager weaponManager;

    void Start()
    {
        weaponManager = GetComponent<PlayerWeaponManager>();
        animator = weaponManager.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) 
        {
            animator.SetTrigger("LightAttack");
        }

        if (Input.GetButtonDown("Fire2")) 
        {
            animator.SetTrigger("HeavyAttack");
        }
    }

    public void StartCombo()
    {
    }

    public void ResetCombo()
    {
    }
}
