using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/Lockpick")]
    public class Lockpick : InventoryItem
    {
        public override bool Use()
        {
            Debug.Log("Lockpick");
            return false;
            //If door return true
        }
    }
}
