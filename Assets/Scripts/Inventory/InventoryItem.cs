using System;
using UnityEngine;
[CreateAssetMenu(menuName = "Item/Item")]
[Serializable]
public class InventoryItem : ScriptableObject
{
    public Sprite sprite = null;
    public Color color = new(1, 1, 1, 255);
    public GameObject HoldObject = null;
    /// <summary>
    /// If the functions returns true that means the use was succesfull and it will be taken out of the hotbar. 
    /// </summary>
    /// <returns></returns>
    public virtual bool Use() { return false; }
}




