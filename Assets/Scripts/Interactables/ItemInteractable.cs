using UnityEngine;
namespace Gameplay
{
    public class ItemInteractable : InteractObject
    {
        [SerializeField] private InventoryItem _item;
        private PlayerInventory _playerInventory;
        public override void Start()
        {
            base.Start();
        }

        public void DoThing()
        {
                _playerInventory = PlrRefs.inst.PlayerInventory;

                if (_playerInventory.IsInventoryFull())
                {
                    return;
                }

                _playerInventory.PickupItem(_item);
                Destroy(gameObject);
        }

    }
}