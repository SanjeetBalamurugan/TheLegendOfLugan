using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public Weapon[] weaponSlots = new Weapon[3];
    private BaseWeapon currentWeapon;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        EquipWeapon(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        if (Input.GetButtonDown("Fire1"))
        {
            currentWeapon?.HandleCombo("Light");
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            currentWeapon?.HandleCombo("Heavy");
        }

        if (Input.GetButtonDown("Fire1"))
        {
            currentWeapon?.Attack();
        }
    }

    void EquipWeapon(int slotIndex)
    {
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

            Debug.Log("Equipped: " + selectedWeapon.weaponName);
        }
    }
}
