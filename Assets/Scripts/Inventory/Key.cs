using UnityEngine;

namespace Gameplay
{
[CreateAssetMenu(menuName = "Item/Key")]
public class Key : InventoryItem
{
    public override bool Use()
    {
        Debug.Log("Key");
            return false;
        //If door return true
    }
}
}
