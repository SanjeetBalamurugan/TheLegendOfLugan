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
            if (colliders.Contains(pickButtons.GetChild(i).GetComponent<Collider>()))
            {
                Destroy(pickButtons.GetChild(i));
            }
        }
    }
}
