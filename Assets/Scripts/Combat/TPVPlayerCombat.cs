using UnityEngine;

public class TPVPlayerCombat : MonoBehaviour
{
    public enum ArrowType
    {
        Physical,
        Pyro,
        Hydro
    }

    [Header("Arrow Settings")]
    public ArrowType currentArrowType = ArrowType.Physical;

    private void Update()
    {
        HandleArrowSwitching();
    }

    private void HandleArrowSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentArrowType = ArrowType.Physical;
            Debug.Log("Switched to Physical Arrow");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentArrowType = ArrowType.Pyro;
            Debug.Log("Switched to Pyro Arrow");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentArrowType = ArrowType.Hydro;
            Debug.Log("Switched to Hydro Arrow");
        }
    }

    public bool HasArrows()
    {
        return ItemManager.instance.GetArrowCount(currentArrowType) > 0;
    }

    public bool TryConsumeArrow()
    {
        return ItemManager.instance.ConsumeArrow(currentArrowType);
    }

    public int GetCurrentArrowCount()
    {
        return ItemManager.instance.GetArrowCount(currentArrowType);
    }
}
