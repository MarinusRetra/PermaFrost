using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Hotbar slot sizes")]
        [SerializeField] private Vector3 _normalSlotSize = new(0.5f, 0.5f, 1f);
        [SerializeField] private Vector3 _selectedSlotSize = new(0.6f, 0.6f, 1.2f);

        [Header("Empty slot prefab here")]
        [SerializeField] private SerializedKeyValuePair<InventoryItem, RectTransform> _emptyItemSlot = new() { Key = null, Value = null };

        [Header("Items that cant be removed")]
        [SerializeField] private List<InventoryItem> _unkillableItems;

        [Header("Inventory item and Hotbarslot transforms")]
        [SerializeField] private List<SerializedKeyValuePair<InventoryItem, RectTransform>> _hotbar; // The key is the item in a slot and the value is a reference to that slot's UI element transform.

        [Header("Reference to input sytem")]
        [SerializeField] private InputReader _input;

        [Header("Debug variables do not assign anything")]
        [SerializeField] private SerializedKeyValuePair<InventoryItem, RectTransform> _selectedHotbarItem;
        public List<SerializedKeyValuePair<InventoryItem, RectTransform>> RemovedHotbarElements = new(); // This is used to remember the removed hotbar slots to re-enable them later.

        void Awake()
        {
            _input.HotbarSelectEvent += HandleHotbarSelect;
            _input.NextPreviousEvent += HandleHotbarNav;
            _input.UseEvent += HandleUse;
            _input.DropEvent += HandleDrop;
            _input.UseEventCancelled += HandleCancelUse;
        }

        void Start()
        {
            // Sets the color and sprite for all hotbar elements and removes all empty inventory slots.
            for (int i = 0; i < _hotbar.Count; i++)
            {
                SetSlotSprite(i);
                if (_hotbar[i].Key == _emptyItemSlot.Key)
                {
                    RemoveSlot(i);
                    i--;
                }
            }
            _selectedHotbarItem = _emptyItemSlot;
        }

        /// <summary>
        /// Uses a float from 0 to 9 and sets the selected inventory slot to that float.
        /// </summary>
        /// (Yes its 0 to 9 even though we only have 5 slots it might get used for other stuff dont worry.)
        private void HandleHotbarSelect(float numberIn)
        {
            int _numberIn = (int)numberIn;

            if (_numberIn > _hotbar.Count - 1 || _hotbar[_numberIn].GetHashCode() == _selectedHotbarItem.GetHashCode())
            {
               _selectedHotbarItem.Value.localScale = _normalSlotSize;
               _selectedHotbarItem = _emptyItemSlot;
                return;
            }

            _selectedHotbarItem.Value.localScale = _normalSlotSize;
            _selectedHotbarItem = _hotbar[_numberIn];
            _hotbar[_numberIn].Value.localScale = _selectedSlotSize;
        }

        /// <summary>
        /// A positive or negative number used to navigate either left or right through the hotbar for scrolling and controller navigation.
        /// </summary>
        private void HandleHotbarNav(float obj)
        {
            //selectedHotbarItem = _hotbar.ElementAt(_hotbar.IndexOf(selectedHotbarItem) + (int)obj);
        }

        /// <summary>
        /// Use item in selected hotbar slot by triggering the item's logic.
        /// </summary>
        private void HandleUse()
        {
            if (_selectedHotbarItem.Key != _emptyItemSlot.Key && _hotbar.Count > 0)
            {
                if (_selectedHotbarItem.Key.Use())
                {
                    RemoveSlot(_hotbar.IndexOf(_selectedHotbarItem));
                    _selectedHotbarItem = _emptyItemSlot;
                }
            }
        }

        private void HandleCancelUse()
        {
            _selectedHotbarItem.Key.UseCancelled();
        }

        /// <summary>
        /// Adds the incoming item to the hotbar if room exists by grabbing the lowest ui element from the removed elements and adding it.
        /// </summary>
        public void PickupItem(InventoryItem incomingItem)
        {
            RemovedHotbarElements[0] = new SerializedKeyValuePair<InventoryItem, RectTransform> { Key = incomingItem, Value = RemovedHotbarElements[0].Value };
            RemovedHotbarElements[0].Value.gameObject.SetActive(true);

            _hotbar.Add(RemovedHotbarElements[0]);
            SetSlotSprite(_hotbar.Count - 1);
            RemovedHotbarElements.RemoveAt(0);

            _hotbar = _hotbar.OrderBy(x => x.Value.gameObject.name[^1]).ToList();
        }

        public void HandleDrop()
        {
            try
            {
                Instantiate(_selectedHotbarItem.Key.HoldObject, transform.position, transform.rotation);
                RemoveSlot(_hotbar.IndexOf(_selectedHotbarItem));
                _selectedHotbarItem = _emptyItemSlot;
            }
            catch { new NullReferenceException(); }
        }

        /// <summary>
        /// Clears out all the slots with an EmptyItem inside.
        /// </summary>
        void  RemoveSlot(int indexIn)
        {
            _hotbar[indexIn].Value.localScale = _normalSlotSize;
            RemovedHotbarElements.Add(_hotbar[indexIn]);
            _hotbar[indexIn].Value.gameObject.SetActive(false);
            _hotbar.RemoveAt(indexIn);
        }

        public bool RemoveSelectedSlot()
        {
            if (_selectedHotbarItem.Key == _emptyItemSlot.Key || _hotbar.Count == 0 || _unkillableItems.Contains(_selectedHotbarItem.Key))
            {
                return false;
            }
            
            _selectedHotbarItem.Value.localScale = _normalSlotSize;
            RemovedHotbarElements.Add(_selectedHotbarItem);
            _selectedHotbarItem.Value.gameObject.SetActive(false);
            _hotbar.RemoveAt(_hotbar.IndexOf(_selectedHotbarItem));
            _selectedHotbarItem = _emptyItemSlot;
            return true;
        }

        /// <summary>
        /// Will set the sprite and color of the passed element at the index of hotbar and set the slot's sprite.
        /// </summary>
        void SetSlotSprite(int indexIn)
        {
            var currentImage = _hotbar[indexIn].Value.GetChild(0).GetComponent<Image>();
            currentImage.sprite = _hotbar[indexIn].Key.sprite;
            currentImage.color = _hotbar[indexIn].Key.color;
        }
        void OnDestroy()
        {
            _input.HotbarSelectEvent -= HandleHotbarSelect;
            _input.NextPreviousEvent -= HandleHotbarNav;
            _input.UseEvent -= HandleUse;
            _input.DropEvent -= HandleDrop;
            _input.UseEventCancelled -= HandleCancelUse;
        }
        void OnDisable()
        {
            _input.DropEvent -= HandleDrop;
            _input.HotbarSelectEvent -= HandleHotbarSelect;
            _input.NextPreviousEvent -= HandleHotbarNav;
            _input.UseEvent -= HandleUse;
            _input.UseEventCancelled -= HandleCancelUse;
        }
	}

    [Serializable]
    public struct SerializedKeyValuePair<TKey, TValue>
    {
        [SerializeField] public TKey Key;
        [SerializeField] public TValue Value;
    }
}
