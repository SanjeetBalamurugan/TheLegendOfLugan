using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public ItemScriptableObject itemData;
    
    public ItemScriptableObject GetItemData()
    {
        return itemData;
    }
}
