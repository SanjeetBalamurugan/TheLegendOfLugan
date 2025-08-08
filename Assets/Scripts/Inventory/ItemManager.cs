using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<ItemScriptableObject> items;
    public static ItemManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<ItemScriptableObject> GetItemScriptableObjects() { return items; }
    public void AddInventoryItems(ItemScriptableObject item)
    {
        items.Add(item);
    }
}
