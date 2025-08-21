using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public Weapon[] weaponSlots = new Weapon[3];
    private BaseWeapon currentWeapon;
    private Animator animator;
    private float cooldownTime = 1f;
    private float timeUntilNextSwitch = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
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
            currentWeapon?.HandleCombo("Light");
            animator.SetTrigger("LightAttack");
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            currentWeapon?.HandleCombo("Heavy");
            animator.SetTrigger("HeavyAttack");
        }
    }

    void EquipWeapon(int slotIndex)
    {
        if (timeUntilNextSwitch > 0) return;

        if (weaponSlots[slotIndex] != null)
        {
            Weapon selectedWeapon = weaponSlots[slotIndex];

            if (selectedWeapon.weaponType == WeaponType.Claymore)
            {
                currentWeapon = gameObject.AddComponent<ClaymoreWeapon>();
                ((ClaymoreWeapon)currentWeapon).Initialize(selectedWeapon, animator);
            }
            else if (selectedWeapon.weaponType == WeaponType.Bow)
            {
                currentWeapon = gameObject.AddComponent<BowWeapon>();
                ((BowWeapon)currentWeapon).Initialize(selectedWeapon, animator);
            }
            else
            {
                currentWeapon = gameObject.AddComponent<SwordWeapon>();
                ((SwordWeapon)currentWeapon).Initialize(selectedWeapon, animator);
            }

            animator.SetInteger("WeaponType", (int)selectedWeapon.weaponType);
            timeUntilNextSwitch = cooldownTime;
        }
    }
}
