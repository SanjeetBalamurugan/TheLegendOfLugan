using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewItem", menuName ="GameName/New Item")]
public class ItemScriptableObject : ScriptableObject
{
    public string Name = "Item Name";
}
