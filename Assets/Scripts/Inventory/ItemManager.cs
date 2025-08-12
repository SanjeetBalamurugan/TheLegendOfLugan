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
        pickButton.GetComponentInChildren<TMP_Text>().text = handler.GetItemData().Name;
        pickButton.transform.GetChild(1).GetComponent<Image>().sprite = handler.GetItemData().itemSprite;
    }
}
