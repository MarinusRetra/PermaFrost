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
        [SerializeField] private List<InventoryItem> _unkillableItems;

        [Header("Inventory item and Hotbarslots")]
        [SerializeField] private InventorySlot[] hotbarSlots;

        [SerializeField] private int currentSelectedSlot_ID = 0;
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
            CurrentSelectedSlot.SlotGameObject.transform.localScale = _normalSlotSize;
            currentSelectedSlot_ID = numberIn;
            CurrentSelectedSlot.SlotGameObject.transform.localScale = _selectedSlotSize;
        }

        /// <summary>
        /// Uses a float from 0 to 9 and sets the selected inventory slot to that float.
        /// </summary>
        /// (Yes its 0 to 9 even though we only have 5 slots it might get used for other stuff dont worry.)
        private void HandleHotbarSelect(float numberIn)
        {
            if (numberIn < HotbarSlots.Length)
            {
                SelectSlot((int)numberIn);
            }
        }

        /// <summary>
        /// A positive or negative number used to navigate either left or right through the hotbar for scrolling and controller navigation.
        /// </summary>
        private void HandleHotbarNav(float numberIn)
        {
            if (CurrentSelectedSlot.Slot_ID-1 > -1 && CurrentSelectedSlot.Slot_ID+1 < hotbarSlots.Length)
            {
                SelectSlot(currentSelectedSlot_ID + (int)numberIn);
            }
        }

        /// <summary>
        /// Use item in selected hotbar slot by triggering the item's logic. Then removes it.
        /// </summary>
        public void HandleUse()
        {
            try
            {
                if (CurrentSelectedSlot.Item.Use())
                {
                    RemoveItemFromSlot(CurrentSelectedSlot.Slot_ID);
                }
            }
            catch (Exception) { }
        }

        public void HandleDrop()
        {
            if (CurrentSelectedSlot.Item != null)
            {
                Instantiate(CurrentSelectedSlot.Item.HoldObject, transform.position, transform.rotation);
                RemoveItemFromSlot(CurrentSelectedSlot.Slot_ID);
            }
        }

        private void HandleCancelUse()
        {
            if (CurrentSelectedSlot.Item == null)
            { 
                CurrentSelectedSlot.Item.UseCancelled();
            }
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
            for (int i = CurrentSelectedSlot.Slot_ID; i < hotbarSlots.Length - 1; i++)
            {
                if (hotbarSlots[i + 1].Item != null)
                {
                    hotbarSlots[i].AddItem(hotbarSlots[i + 1].Item);
                    hotbarSlots[i + 1].ClearSlot();
                }
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
