using UnityEngine;
namespace Gameplay
{
    public class ItemInteractable : InteractObject
    {
        [SerializeField] private InventoryItem _item;
        private GameObject _player;
        public override void Start()
        {
            base.Start();
            _player = PlayerStatusEffects.Instance.gameObject;
            InteractEvent.AddListener(() =>
            {
                _player.GetComponent<PlayerInventory>().PickupItem(_item);
                Destroy(gameObject);
            });
        }
    }
}