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
[CreateAssetMenu(menuName = "Item/HeatPack")]

public class HeatPack : InventoryItem
{
    public override bool Use()
    {
        Debug.Log("Heatpack");
        return true;
    }
}
[CreateAssetMenu(menuName = "Item/NoiseMonkey")]

public class NoiseMonkey : InventoryItem
{
    public override bool Use()
    {
        Debug.Log("NoiseMonkey");
        return true;
    }
}
[CreateAssetMenu(menuName = "Item/SpeedSyringe")]

public class SpeedSyringe : InventoryItem
{
    public override bool Use()
    {
        Debug.Log("SpeedSyringe");
        return true;
    }
}
[CreateAssetMenu(menuName = "Item/Key")]
public class Key : InventoryItem
{
    public override bool Use()
    {
        Debug.Log("Key");
        return true;
    }
}
[CreateAssetMenu(menuName = "Item/Bell")]
public class Bell : InventoryItem
{
    public override bool Use()
    {
        Debug.Log("Bell");
        return true;
    }
}
[CreateAssetMenu(menuName = "Item/Lockpick")]
public class Lockpick : InventoryItem
{
    public override bool Use()
    {
        Debug.Log("Lockpick");
        return true;
    }
}




