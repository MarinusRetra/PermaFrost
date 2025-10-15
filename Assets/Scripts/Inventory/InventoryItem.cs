using UnityEngine;
[CreateAssetMenu(menuName = "Item/Item")]
public class InventoryItem : ScriptableObject
{
    public Sprite sprite = null;
    public Color color = new Color(1,1,1,255);
    public GameObject HoldObject = null;
    public virtual void Use() { }

    public InventoryItem()
    {
        SetColorAndSprite(color, sprite);
    }

    public void SetColorAndSprite(Color colorIn, Sprite sprinteIn)
    {
        color = colorIn;
        sprite = sprinteIn;
    }
}
[CreateAssetMenu(menuName = "Item/HeatPack")]

public class HeatPack : InventoryItem
{
    public HeatPack() 
    {
        Items.AllItems.Add(this);
        Debug.Log(ToString());
    }
    public override void Use()
    {
        //
    }
}
[CreateAssetMenu(menuName = "Item/NoiseMonkey")]

public class NoiseMonkey : InventoryItem
{
    public NoiseMonkey()
    { Items.AllItems.Add(this); }

    public override void Use()
    {
        //
    }
}
[CreateAssetMenu(menuName = "Item/SpeedSyringe")]

public class SpeedSyringe : InventoryItem
{
    public SpeedSyringe() 
    { Items.AllItems.Add(this); }

    public override void Use()
    {
        //
    }
}
[CreateAssetMenu(menuName = "Item/Key")]
public class Key : InventoryItem
{
    public Key() 
    { Items.AllItems.Add(this); }

    public override void Use()
    {
        //
    }
}
[CreateAssetMenu(menuName = "Item/Bell")]
public class Bell : InventoryItem
{
    public Bell() 
    { Items.AllItems.Add(this); }

    public override void Use()
    {
        //
    }
}
[CreateAssetMenu(menuName = "Item/LockPick")]
public class Lockpick : InventoryItem
{
    public Lockpick() 
    { Items.AllItems.Add(this); }

    public override void Use()
    {
        //
    }
}

