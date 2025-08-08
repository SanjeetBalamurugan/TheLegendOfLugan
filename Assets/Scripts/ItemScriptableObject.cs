using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Consumable,
    Misc
}

[CreateAssetMenu(fileName ="NewItem", menuName ="GameName/New Item")]
public class ItemScriptableObject : ScriptableObject
{
    public string Name = "Item Name";
    public ItemType type = ItemType.Misc;
    public Sprite itemSprite;
    public GameObject itemPrefab;
}
