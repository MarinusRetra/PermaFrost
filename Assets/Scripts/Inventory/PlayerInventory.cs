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

        [Header("Items that cant be removed")]
        [SerializeField] private InventoryItem[] _unkillableItems;

        [Header("Inventory item and Hotbarslots")]
        [SerializeField] private InventorySlot[] hotbarSlots;

        [SerializeField] private int currentSelectedSlot_ID = -1;
        public InventorySlot CurrentSelectedSlot { get => hotbarSlots[currentSelectedSlot_ID]; }
        public InventorySlot[] HotbarSlots { get => hotbarSlots; }

        [Header("Reference to input sytem")]
        [SerializeField] private InputReader _input;

        public void Awake()
        {
            _input.HotbarSelectEvent += HandleHotbarSelect;
            _input.NextPreviousEvent += HandleHotbarNav;
            _input.UseEvent += HandleUse;
            _input.DropEvent += HandleDrop;
            _input.UseEventCancelled += HandleCancelUse;
        }

        private void SelectSlot(int numberIn)
        {
            if (currentSelectedSlot_ID == numberIn)
            { 
                DeselectSlots();
                return;
            }
            else if (currentSelectedSlot_ID == -1)
            {
                currentSelectedSlot_ID = numberIn;
                CurrentSelectedSlot.SlotGameObject.transform.localScale = _selectedSlotSize;
                return;
            }

            if (numberIn < hotbarSlots.Length)
            { 
                Debug.Log("Normal select");
                CurrentSelectedSlot.SlotGameObject.transform.localScale = _normalSlotSize;
                currentSelectedSlot_ID = numberIn;
                CurrentSelectedSlot.SlotGameObject.transform.localScale = _selectedSlotSize;
            }
        }

        private void DeselectSlots()
        {
            CurrentSelectedSlot.SlotGameObject.transform.localScale = _normalSlotSize;
            currentSelectedSlot_ID = -1;
        }

        /// <summary>
        /// Uses a float from 0 to 9 and sets the selected inventory slot to that float.
        /// </summary>
        /// (Yes its 0 to 9 even though we only have 5 slots it might get used for other stuff dont worry.)
        private void HandleHotbarSelect(float numberIn)
        {
            SelectSlot((int)numberIn);
        }

        /// <summary>
        /// A positive or negative number used to navigate either left or right through the hotbar for scrolling and controller navigation.
        /// </summary>
        private void HandleHotbarNav(float numberIn)
        {
            if (currentSelectedSlot_ID == -1)
            { 
                SelectSlot(numberIn == -1 ? GetItemsInInventory()-1 : 0);
                return;
            }
            if (CurrentSelectedSlot.Slot_ID + numberIn < 0 || CurrentSelectedSlot.Slot_ID + numberIn > GetItemsInInventory()-1)
            {
                return;
            }
            SelectSlot(currentSelectedSlot_ID + (int)numberIn);
        }

        private int GetItemsInInventory()
        {
            int i = 0;
            foreach (var slot in hotbarSlots)
            {
                if (slot.Item != null)
                {
                    i++;
                }
            }
            return i;
        }

        /// <summary>
        /// Use item in selected hotbar slot by triggering the item's logic. Then removes it.
        /// </summary>
        public void HandleUse()
        {
            try
            {
                if (currentSelectedSlot_ID != -1 || CurrentSelectedSlot.Item != null)
                { 
                    if (CurrentSelectedSlot.Item.Use())
                    {
                        RemoveItemFromSlot(currentSelectedSlot_ID);
                    }
                }
            }
            catch (Exception) { }
        }

        public void HandleDrop()
        {
            if (currentSelectedSlot_ID != -1 || CurrentSelectedSlot.Item != null)
            {
                Instantiate(CurrentSelectedSlot.Item.HoldObject, transform.position, transform.rotation);
                RemoveItemFromSlot(CurrentSelectedSlot.Slot_ID);
            }
        }

        private void HandleCancelUse()
        {
            try
            { 
                CurrentSelectedSlot.Item.UseCancelled();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Adds the incoming item to the hotbar if room for it exists.
        /// </summary>
        public void PickupItem(InventoryItem incomingItem)
        {
            foreach (InventorySlot slot in hotbarSlots)
            {
                if (slot.Item == null)
                { 
                    slot.AddItem(incomingItem);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes the item and makes all the items next to it shift one to the left to prevent gaps in the inventory.
        /// </summary>
        /// <param name="currentSlotIn"></param>
        public void RemoveItemFromSlot(int currentSlotIn)
        {
            CurrentSelectedSlot.ClearSlot();
            for (int i = currentSelectedSlot_ID; i < hotbarSlots.Length; i++)
            {
                if (hotbarSlots[i + 1].Item != null)
                {
                    hotbarSlots[i].AddItem(hotbarSlots[i + 1].Item);
                    hotbarSlots[i + 1].ClearSlot();
                }
            }
            if (GetItemsInInventory() == 0)
            { 
                DeselectSlots();
            }
        }

        public bool IsInventoryFull()
        {
            int i = 0;
            foreach (var slot in hotbarSlots)
            {
                if (slot.Item != null)
                {
                    i++;
                }
            }

            if (i == hotbarSlots.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
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
    public class InventorySlot
    {
        [SerializeField] public int Slot_ID;
        [SerializeField] public GameObject SlotGameObject;
        [SerializeField] public Image Slot_Image;
        [SerializeField] public InventoryItem Item;

        public void ClearSlot()
        {
            UpdateSprite();
            SlotGameObject.SetActive(false);
            Item = null;
        }

        public void UpdateSprite()
        {
            Slot_Image.sprite = Item.sprite;
            Slot_Image.color = Item.color;
        }

        public void AddItem(InventoryItem itemIn)
        {
            Item = itemIn;
            UpdateSprite();
            SlotGameObject.SetActive(true);
        }

    }
}
