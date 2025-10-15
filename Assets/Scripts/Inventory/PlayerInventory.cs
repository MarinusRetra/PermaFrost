using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "PlayerInventory", menuName = "Scriptable Objects/PlayerInventory")]
    public class PlayerInventory : ScriptableObject
    {
        private List<InventoryItem> _hotbar = new List<InventoryItem>();
        private InventoryItem selectedHotbarItem;
        [SerializeField] InputReader _input;

        void Awake()
        {
            _input.HotbarSelectEvent += HandleHotbarSelect;
            _input.NextPreviousEvent += HandleHotbarNav;
            _input.UseEvent += HandleUse;
        }

        /// <summary>
        /// Uses a float from 0 to 9 and sets the selected inventory slot to that float.
        /// </summary>
        private void HandleHotbarSelect(float number)
        {
            selectedHotbarItem = _hotbar[(int)number];
            // Zet ook ui selected naar geselecteerde item.
        }

        /// <summary>
        /// A positive or negative number used to navigate either left or right through the hotbar for scrolling and controller navigation.
        /// </summary>
        private void HandleHotbarNav(float obj)
        {
            selectedHotbarItem = _hotbar.ElementAt(_hotbar.IndexOf(selectedHotbarItem) + (int)obj);
        }

        public void PickupItem(string itemIn)
        {
            _hotbar.Add(Items.FindInventoryItem(itemIn));
            
        }

        /// <summary>
        /// Use item in selected hotbar slot by triggering the item's logic.
        /// </summary>
        private void HandleUse()
        {
            selectedHotbarItem.Use();
            _hotbar.Remove(selectedHotbarItem);
        }

        public void DropItem()
        {
            _hotbar.Remove(selectedHotbarItem);
            // Instantiate item
        }

        void OnDestroy()
        {
            _input.HotbarSelectEvent -= HandleHotbarSelect;
            _input.NextPreviousEvent -= HandleHotbarNav;
            _input.UseEvent -= HandleUse;
        }
        void OnDisable()
        {
            _input.HotbarSelectEvent -= HandleHotbarSelect;
            _input.NextPreviousEvent -= HandleHotbarNav;
            _input.UseEvent -= HandleUse;
        }
    }
}
