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
    public int pyroArrowCount = 10;
    public int hydroArrowCount = 10;
    public int physicalArrowCount = 20;

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
}
