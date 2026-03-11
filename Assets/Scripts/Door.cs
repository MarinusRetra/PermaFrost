using UnityEngine;

namespace Gameplay
{
    public class Door : MonoBehaviour
    {
        public InventoryItem key;
        

        public void CheckAndUseKey()
        {
            if(FindAnyObjectByType<PlayerInventory>()._selectedHotbarItem.Key == key)
            {
                FindAnyObjectByType<PlayerInventory>().HandleUse();
            }
        }
    }
}
