using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/Key")]
    public class Key : InventoryItem
    {
        public override bool Use()
        {
            GameObject lookinAt = PlrRefs.inst.Interactor.hit.collider?.gameObject;
            if (lookinAt && lookinAt.CompareTag("Door"))
            {
                PlrRefs.inst.PlayerInventory.HandleUseAnimation();
                Destroy(lookinAt);
                return true;
            }
            return false;
        }
    }
}
