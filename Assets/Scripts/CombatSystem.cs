using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private BaseWeapon currentWeapon;
    private float comboTimer = 0f;
    private float comboTimeLimit = 0.5f;
    private string bufferedInput;

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
        currentWeapon?.StartCombo();
    }

    public void HandleComboInput(string input)
    {
        if (comboTimer > 0f)
        {
            currentWeapon?.HandleCombo(input);
            comboTimer = comboTimeLimit;
        }
        else
        {
            bufferedInput = input;
        }
    }

    public void Attack(string input)
    {
        if (currentWeapon == null) return;
        currentWeapon.Attack();
        comboTimer = comboTimeLimit;
    }

    void Update()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f && !string.IsNullOrEmpty(bufferedInput))
            {
                currentWeapon?.HandleCombo(bufferedInput);
                bufferedInput = null;
                comboTimer = comboTimeLimit;
            }
        }
    }
}
