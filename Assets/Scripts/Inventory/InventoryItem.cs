using UnityEngine;
[CreateAssetMenu(menuName = "Item/Item")]
public class InventoryItem : ScriptableObject
{
    public Sprite sprite = null;
    public Color color = new(1,1,1,255);
    public GameObject HoldObject = null;
    public virtual void Use() { }
    public InventoryItem ReturnSelf()
    {
        return this;
    }
}
[CreateAssetMenu(menuName = "Item/HeatPack")]

public class HeatPack : InventoryItem
{
    public override void Use()
    {
        //
    }
}
[CreateAssetMenu(menuName = "Item/NoiseMonkey")]

public class NoiseMonkey : InventoryItem
{
    public override void Use()
    {
        //
    }
}
[CreateAssetMenu(menuName = "Item/SpeedSyringe")]

public class SpeedSyringe : InventoryItem
{
    public override void Use()
    {
        //
    }
}
[CreateAssetMenu(menuName = "Item/Key")]
public class Key : InventoryItem
{
    public override void Use()
    {
        //
    }
}
[CreateAssetMenu(menuName = "Item/Bell")]
public class Bell : InventoryItem
{
    public override void Use()
    {
        //
    }
}
[CreateAssetMenu(menuName = "Item/LockPick")]
public class Lockpick : InventoryItem
{
    public override void Use()
    {
        //
    }
}

