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
            _playerInventory = PlayerStatusEffects.Instance.gameObject.GetComponent<PlayerInventory>();
        }

        public void DoThing()
        {
            if (_playerInventory.CurrentSelectedSlot.Slot_ID > _playerInventory.HotbarSlots.Length-1)
            {
                return;
            }
            _playerInventory.PickupItem(_item);
            Destroy(gameObject);
        }
    }
}