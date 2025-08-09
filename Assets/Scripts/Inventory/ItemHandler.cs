using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public static ItemScriptableObject itemData;
    
    public ItemScriptableObject GetItemData()
    {
        return itemData;
    }
}
