using UnityEngine;

public class TPVPlayerCombat : MonoBehaviour
{
    public enum ArrowType { Pyro, Hydro, Physical }
    public ArrowType currentArrowType = ArrowType.Physical;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentArrowType = ArrowType.Pyro;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentArrowType = ArrowType.Hydro;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentArrowType = ArrowType.Physical;
    }

    public bool HasArrows()
    {
        return GetArrowCount(currentArrowType) > 0;
    }

    public bool TryConsumeArrow()
    {
        switch (currentArrowType)
        {
            case ArrowType.Pyro:
                if (ItemManager.instance.pyroArrowCount > 0)
                {
                    ItemManager.instance.pyroArrowCount--;
                    return true;
                }
                break;
            case ArrowType.Hydro:
                if (ItemManager.instance.hydroArrowCount > 0)
                {
                    ItemManager.instance.hydroArrowCount--;
                    return true;
                }
                break;
            case ArrowType.Physical:
                if (ItemManager.instance.physicalArrowCount > 0)
                {
                    ItemManager.instance.physicalArrowCount--;
                    return true;
                }
                break;
        }
        return false;
    }

    private int GetArrowCount(ArrowType type)
    {
        switch (type)
        {
            case ArrowType.Pyro: return ItemManager.instance.pyroArrowCount;
            case ArrowType.Hydro: return ItemManager.instance.hydroArrowCount;
            case ArrowType.Physical: return ItemManager.instance.physicalArrowCount;
        }
        return 0;
    }
}
