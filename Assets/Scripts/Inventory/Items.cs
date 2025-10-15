
public static class Items
{
    public static InventoryItem[] AllItems;

    public static InventoryItem FindInventoryItem(string itemName)
    {
        foreach (var item in AllItems)
        {
            if (itemName == item.ToString())
            {
                return item;
            }
        }
        return null;
    }
}
