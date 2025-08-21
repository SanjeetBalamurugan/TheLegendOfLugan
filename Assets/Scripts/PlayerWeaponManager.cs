using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public Weapon[] weaponSlots = new Weapon[3];
    private BaseWeapon currentWeapon;
    private Animator animator;
    private float cooldownTime = 1f;
    private float timeUntilNextSwitch = 0f;
    private CombatSystem combatSystem;

    void Start()
    {
        animator = GetComponent<Animator>();
        combatSystem = GetComponent<CombatSystem>();
        EquipWeapon(0);
    }

    void Update()
    {
        if (timeUntilNextSwitch > 0)
        {
            timeUntilNextSwitch -= Time.deltaTime;
        }

        if (timeUntilNextSwitch <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            combatSystem.HandleComboInput("Fire1");
            animator.SetTrigger("LightAttack");
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            combatSystem.HandleComboInput("Fire2");
            animator.SetTrigger("HeavyAttack");
        }
    }

    void EquipWeapon(int slotIndex)
    {
        if (timeUntilNextSwitch > 0) return;

        if (weaponSlots[slotIndex] != null)
        {
            Weapon selectedWeapon = weaponSlots[slotIndex];

            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }

            if (selectedWeapon.weaponType == WeaponType.Claymore)
            {
                currentWeapon = new ClaymoreWeapon();
            }
            else if (selectedWeapon.weaponType == WeaponType.Bow)
            {
                currentWeapon = new BowWeapon();
            }
            else
            {
                currentWeapon = new SwordWeapon();
            }

            currentWeapon.Initialize(selectedWeapon, animator);
            combatSystem.Initialize(currentWeapon);
            animator.SetInteger("WeaponType", (int)selectedWeapon.weaponType);
            timeUntilNextSwitch = cooldownTime;
        }
    }
}
