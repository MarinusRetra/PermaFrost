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

        [SerializeField] private InventorySlot currentSelectedSlot;

        public InventorySlot CurrentSelectedSlot { get => currentSelectedSlot; }
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

        void Start()
        {
            currentSelectedSlot.Item = null;
        }


        private void SelectSlot(int numberIn)
        {
            int _numberIn = (int)numberIn;
            currentSelectedSlot.SlotGameObject.transform.localScale = _normalSlotSize;
            currentSelectedSlot = hotbarSlots[_numberIn];
            currentSelectedSlot.SlotGameObject.transform.localScale = _selectedSlotSize;
        }

        /// <summary>
        /// Uses a float from 0 to 9 and sets the selected inventory slot to that float.
        /// </summary>
        /// (Yes its 0 to 9 even though we only have 5 slots it might get used for other stuff dont worry.)
        private void HandleHotbarSelect(float numberIn)
        {
            if (numberIn < hotbarSlots.Length - 1)
            {
                SelectSlot((int)numberIn);
            }
        }

        /// <summary>
        /// A positive or negative number used to navigate either left or right through the hotbar for scrolling and controller navigation.
        /// </summary>
        private void HandleHotbarNav(float numberIn)
        {
            if (currentSelectedSlot.Slot_ID-1 > -1 && currentSelectedSlot.Slot_ID+1 < hotbarSlots.Length - 1)
            {
                SelectSlot(currentSelectedSlot.Slot_ID + (int)numberIn);
            }
        }

        /// <summary>
        /// Use item in selected hotbar slot by triggering the item's logic.
        /// </summary>
        public void HandleUse()
        {
            currentSelectedSlot.Item?.Use();
        }

        private void HandleCancelUse()
        {
            currentSelectedSlot.Item?.UseCancelled();
        }

        /// <summary>
        /// Adds the incoming item to the hotbar if room for it exists.
        /// </summary>
        public void PickupItem(InventoryItem incomingItem)
        {
            currentSelectedSlot.AddItem(ref incomingItem);
        }

        public void HandleDrop()
        {
            try
            {
                Instantiate(currentSelectedSlot.Item.HoldObject, transform.position, transform.rotation);
                currentSelectedSlot.ClearSlot();
            }
            catch { new NullReferenceException(); }
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
    public struct InventorySlot
    {
        [SerializeField] public int Slot_ID;
        [SerializeField] public GameObject SlotGameObject;
        [SerializeField] public Image Slot_Image;
        [SerializeField] public InventoryItem Item;

        public void ClearSlot()
        {
            SlotGameObject.SetActive(false);
            Item = null;
        }

        public void SetSprite()
        {
            Slot_Image.sprite = Item.sprite;
            Slot_Image.color = Item.color;
        }

        public void AddItem(ref InventoryItem itemIn)
        {
            Item = itemIn;
            SetSprite();
            SlotGameObject.SetActive(true);
        }

    }
}
