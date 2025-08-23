using UnityEngine;

public class TPVPlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BowWeapon bowWeapon;

    [Header("Arrow Settings")]
    public ArrowType currentArrowType = ArrowType.Physical;

    public enum ArrowType
    {
        Physical,
        Pyro,
        Hydro
    }

    void Update()
    {
        if (bowWeapon != null)
            bowWeapon.HandleCombat(currentArrowType);
    }
}
