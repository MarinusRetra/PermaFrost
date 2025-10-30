using System;
using System.Collections.Generic;
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
            for (int i = 0; i < _hotbar.Count; i++)
            {
                CorrectUIValue(i);
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

        /// <summary>
        /// Adds the incoming item to the hotbar if room exists by grabbing the lowest ui element from the removed elements and adding it.
        /// </summary>
        public void PickupItem(InventoryItem incomingItem)
        {
            if (_removedHotbarElements.Count == 0)
            {
                return;
            }

            _removedHotbarElements[0] = new SerializedKeyValuePair<InventoryItem, RectTransform> { Key = incomingItem, Value = _removedHotbarElements[0].Value };
            _hotbar.Add(_removedHotbarElements[0]);

            int lastHotbarElement = _hotbar.Count-1;//_hotbar.IndexOf(_removedHotbarElements[0]);

            // Kan als het goed is ook gewoon het laatste element van de _hotbar pakken
            CorrectUIValue(lastHotbarElement);
            _removedHotbarElements.Remove(_removedHotbarElements[0]);

            // Bubbles one item to the end of the list. Only one item is added each time so only one pass is ever needed
            // for (var i = 1; i < _hotbar.Count; i++)
            // {
            //     if (_hotbar[i - 1].Value.name[^1] > _hotbar[i].Value.name[^1])
            //     {
            //         (_hotbar[i], _hotbar[i - 1]) = (_hotbar[i - 1], _hotbar[i]);
            //     }
            // }


        }
        /// <summary>
        /// Sets the ui element to the index passed so the index matches with the value of the inventory item.
        /// </summary>
        void CorrectUIValue(int indexIn)
        {
            //Save _hotbar[indexIn] value to swap later
            //Grab ui element matching the index
            //Set _hotbar[indexIn].Value to the matched ui element
            //Set the element behind the matched ui element from _removedHotbarElements[indexIn] to the saved hotbar variable

            var swapVar = _hotbar[indexIn];
            foreach (var pair in _removedHotbarElements)
            {

                //If UI element from _removedHotbarElements matches index from provided index swap them
                if (pair.Value.name[^1] - 48 == indexIn)
                {
                    swapVar.Value.gameObject.SetActive(false);
                    _hotbar[indexIn] = new SerializedKeyValuePair<InventoryItem, RectTransform> { Key = _hotbar[indexIn].Key, Value = pair.Value };
                    _removedHotbarElements[_removedHotbarElements.IndexOf(pair)] = new SerializedKeyValuePair<InventoryItem, RectTransform> { Key = _emptyItemSlot.Key, Value = swapVar.Value };
                    _hotbar[indexIn].Value.gameObject.SetActive(true);
                    break;
                }
            }

            //If current hotbar index does not match its UI element swap them with the matching element
            for (var i = 0; i < _hotbar.Count; i++)
            {
                if (_hotbar[i].Value.name[^1] - 48 != i)
                {
                    foreach (var hotbarPair in _hotbar)
                    {
                        if (hotbarPair.Value.name[^1] - 48 == i)
                        {
                            Debug.Log($"Index: {i} current hotbar transform name: {_hotbar[i].Value.gameObject.name}, currentPair {hotbarPair.Value.name}");
                            _hotbar[i].Value.gameObject.SetActive(false);
                            _hotbar[i] = hotbarPair;
                            _hotbar[_hotbar.IndexOf(hotbarPair)] = swapVar;
                            _hotbar[i].Value.gameObject.SetActive(true);
                            Debug.Log($"Index: {i} current hotbar transform name: {_hotbar[i].Value.gameObject.name}, currentPair{hotbarPair.Value.name}");
                            SetSlotSprite(i);
                            SetSlotSprite(_hotbar.IndexOf(hotbarPair));
                            break;
                        }
                    }
                }
            }
            SetSlotSprite(indexIn);
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
            _hotbar[indexIn].Value.localScale = _normalSlotSize;
            _removedHotbarElements.Add(_hotbar[indexIn]);
            _hotbar[indexIn].Value.gameObject.SetActive(false);
            _hotbar.RemoveAt(indexIn);
            
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
