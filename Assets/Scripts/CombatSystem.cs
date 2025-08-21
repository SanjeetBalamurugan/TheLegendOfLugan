using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private BaseWeapon currentWeapon;
    private string inputBuffer = "";
    private float comboTimer = 0f;
    private float comboTimeLimit = 0.5f;

    public void Initialize(BaseWeapon weapon)
    {
        currentWeapon = weapon;
    }

    public void SwitchWeapon(BaseWeapon newWeapon)
    {
        currentWeapon = newWeapon;
    }

    public void StartCombo()
    {
        comboTimer = comboTimeLimit;
        currentWeapon.StartCombo();
    }

    public void HandleComboInput(string input)
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            currentWeapon.HandleCombo(input);
        }
        else
        {
            StartCombo();  // Restart combo if time is up
        }
    }

    public void Attack()
    {
        if (currentWeapon != null)
        {
            currentWeapon.Attack();
        }
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            HandleComboInput("Fire1");
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            HandleComboInput("Fire2");
        }
    }
}
