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
        [SerializeField] private SerializedKeyValuePair<InventoryItem, RectTransform> _emptyItemSlot;

        [Header("Inventory item and Hotbarslot transforms")]
        [SerializeField] private List<SerializedKeyValuePair<InventoryItem, RectTransform>> _hotbar; // The key is the item in a slot and the value is a reference to that slot's UI element transform.

        [Header("Reference to input sytem")]
        [SerializeField] private InputReader _input;
        
        [Header("Debug variables do not assign anything")]
        [SerializeField] private SerializedKeyValuePair<InventoryItem, RectTransform> _selectedHotbarItem;
        [SerializeField] private List<SerializedKeyValuePair<InventoryItem, RectTransform>> _removedHotbarElements = new(); // This is used to remember the removed hotbar slots to re-enable them later.

        void Awake()
        {
            _input.HotbarSelectEvent += HandleHotbarSelect;
            _input.NextPreviousEvent += HandleHotbarNav;
            _input.UseEvent += HandleUse;
        }

        void Start()
        {
            // Sets the color and sprite for all hotbar elements.
            // Removes all empty inventory slots.
            for (int i = 0; i < _hotbar.Count; i++)
            {
                var currentImage = _hotbar[i].Value.GetChild(0).GetComponent<Image>();
                currentImage.sprite = _hotbar[i].Key.sprite;
                currentImage.color = _hotbar[i].Key.color;
                if (_hotbar[i].Key == _emptyItemSlot.Key)
                {
                    RemoveSlot(i);
                    i--;
                }
            }
            _selectedHotbarItem = _hotbar.First();
            _selectedHotbarItem.Value.localScale = _selectedSlotSize;
        }

        /// <summary>
        /// Uses a float from 0 to 9 and sets the selected inventory slot to that float.
        /// </summary>
        private void HandleHotbarSelect(float numberIn)
        {
            int _numberIn = (int)numberIn;
            if (numberIn > _hotbar.Count - 1)
            {
                return;
            }
            // Als removed elements geen selected slot heeft zet dan de grootte terug naar normaal
            if (!_removedHotbarElements.Contains(_selectedHotbarItem))
            {
                _selectedHotbarItem.Value.localScale = _normalSlotSize;
            }
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
        /// Als er een element in _removedHotbarElements zit dan voegt 
        /// </summary>
        /// <param name="itemIn"></param>
        public void PickupItem(string itemIn)
        {
            try { _hotbar.Add(_removedHotbarElements[0]); }
            catch
            {
                Debug.Log("Cannot exceed max inventory size");
                return;
            }
            //_hotbar.Add(IncomingInventoryItem, _removedHotbarElements[0].Value);
            _removedHotbarElements.RemoveAt(0);
            SetSelectedSlotSprite();
        }

        /// <summary>
        /// Use item in selected hotbar slot by triggering the item's logic.
        /// </summary>
        private void HandleUse()
        {
            if (_selectedHotbarItem.Key != _emptyItemSlot.Key && _hotbar.Count > 0)
            {
                _selectedHotbarItem.Key.Use();
                RemoveSlot(_hotbar.IndexOf(_selectedHotbarItem));
                _selectedHotbarItem = _emptyItemSlot;
            }
        }

        public void DropItem()
        {
            // Instantiate item
            RemoveSlot(_hotbar.IndexOf(_selectedHotbarItem));
        }
        /// <summary>
        /// Clears out all the slots with an EmptyItem inside.
        /// </summary>
        void RemoveSlot(int indexIn)
        {
            _removedHotbarElements.Add(_hotbar[indexIn]);
            _hotbar[indexIn].Value.gameObject.SetActive(false);
            _hotbar.RemoveAt(indexIn);
            
            // foreach (var item in _removedHotbarElements)
            // {
            //     item.Value.gameObject.SetActive(false);
            // }
        }

        /// <summary>
        /// Will set the sprite and color of the selected slot to the sprite and color of the contained item.
        /// </summary>
        void SetSelectedSlotSprite()
        {
            var currentImage = _selectedHotbarItem.Value.GetChild(0).GetComponent<Image>();
            currentImage.sprite = _selectedHotbarItem.Key.sprite;
            currentImage.color = _selectedHotbarItem.Key.color;
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


[Serializable]
    public struct SerializedKeyValuePair<TKey, TValue>
    {
        [SerializeField] public TKey Key;
        [SerializeField] public TValue Value;
    }
}
