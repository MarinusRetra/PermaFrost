using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Image Plaatje;
    public GameObject HoldObject;
    public virtual void Use() { }
}

public class HeatPack : InventoryItem
{
    public override void Use()
    {
        //
    }
}
public class NoiseMonkey : InventoryItem
{
    public override void Use()
    {
        //
    }
}
public class SpeedSyringe : InventoryItem
{
    public override void Use()
    {
        //
    }
}
public class Key : InventoryItem
{
    public override void Use()
    {
        //
    }
}
public class Bell : InventoryItem
{
    public override void Use()
    {
        //
    }
}
public class Lockpick : InventoryItem
{
    public override void Use()
    {
        //
    }
}

