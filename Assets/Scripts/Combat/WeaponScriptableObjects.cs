using UnityEngine;

public enum WeaponType
{
    Sword,
    Claymore,
    Bow,
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon")]
public class WeaponScriptableObjects : ScriptableObject
{
    public WeaponType weaponType;
    public string weaponName;
    public int damage;
    public float attackSpeed;
    public float range;
    public string attackAnimationTrigger;
}
