using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemManager : MonoBehaviour
{
    public List<ItemScriptableObject> items;
    public static ItemManager instance;
    public Transform pickButtons;
    public GameObject pickButtonPrefab;
    public List<Collider> picked;

    [Header("Arrow Inventory")]
    public int physicalArrowCount = 0;
    public int pyroArrowCount = 0;
    public int hydroArrowCount = 0;

    private void Awake()
    {
        instance = this;
    }

    public List<ItemScriptableObject> GetItemScriptableObjects() { return items; }

    public void AddInventoryItems(ItemScriptableObject item)
    {
        items.Add(item);
    }

    public void PickUpItem(Collider collider)
    {
        ItemHandler handler = collider.GetComponent<ItemHandler>();
        GameObject pickButton = Instantiate(pickButtonPrefab, pickButtons);
        pickButton.name = handler.itemData.name + " - Button";
        pickButton.GetComponentInChildren<TMP_Text>().text = handler.GetItemData().Name;
        pickButton.transform.GetChild(1).GetComponent<Image>().sprite = handler.GetItemData().itemSprite;
    }

    public void AfterPickUp(List<Collider> colliders)
    {
        for (var i = 0; i < pickButtons.childCount; i++)
        {
            Transform buttonTransform = pickButtons.GetChild(i);
            Collider buttonCollider = buttonTransform.GetComponent<Collider>();

            if (buttonCollider != null && colliders.Contains(buttonCollider))
            {
                Destroy(buttonTransform.gameObject);
            }
        }
    }

    public int GetArrowCount(TPVPlayerCombat.ArrowType type)
    {
        switch (type)
        {
            case TPVPlayerCombat.ArrowType.Physical: return physicalArrowCount;
            case TPVPlayerCombat.ArrowType.Pyro: return pyroArrowCount;
            case TPVPlayerCombat.ArrowType.Hydro: return hydroArrowCount;
            default: return 0;
        }
    }

    public void AddArrows(TPVPlayerCombat.ArrowType type, int amount)
    {
        switch (type)
        {
            case TPVPlayerCombat.ArrowType.Physical: physicalArrowCount += amount; break;
            case TPVPlayerCombat.ArrowType.Pyro: pyroArrowCount += amount; break;
            case TPVPlayerCombat.ArrowType.Hydro: hydroArrowCount += amount; break;
        }
    }

    public bool ConsumeArrow(TPVPlayerCombat.ArrowType type)
    {
        switch (type)
        {
            case TPVPlayerCombat.ArrowType.Physical:
                if (physicalArrowCount > 0) { physicalArrowCount--; return true; }
                break;
            case TPVPlayerCombat.ArrowType.Pyro:
                if (pyroArrowCount > 0) { pyroArrowCount--; return true; }
                break;
            case TPVPlayerCombat.ArrowType.Hydro:
                if (hydroArrowCount > 0) { hydroArrowCount--; return true; }
                break;
        }
        return false;
    }
}
