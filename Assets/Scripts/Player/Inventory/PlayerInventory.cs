using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class PlayerInventory : MonoBehaviour
    {
        //These get used for setting the animations in the current selected slot when selecting to and from it.
        private static readonly int SelectingHash = Animator.StringToHash("Selecting");
        private static readonly int DeselectingHash = Animator.StringToHash("Deselecting");
        private static readonly int SelectedHash = Animator.StringToHash("Selected");
        
        [Header("Hotbar slot sizes")]
        [SerializeField] private Vector3 _normalSlotSize = new(0.5f, 0.5f, 1f);
        [SerializeField] private Vector3 _selectedSlotSize = new(0.6f, 0.6f, 1.2f);

        [Header("Items that cant be removed")]
        [SerializeField] private InventoryItem[] _unkillableItems;

        [Header("Inventory item and Hotbarslots")]
        [SerializeField] private InventorySlot[] hotbarSlots;
        [SerializeField] private int currentSelectedSlot_ID = -1;
        public InventorySlot CurrentSelectedSlot { get => hotbarSlots[currentSelectedSlot_ID]; }

        [Header("Reference to input sytem")]
        [SerializeField] private InputReader _input;

        public Animator playerArmAnimator;

        public void Awake()
        {
            _input.HotbarSelectEvent += HandleHotbarSelect;
            _input.NextPreviousEvent += HandleHotbarNav;
            _input.UseEvent += HandleUse;
            _input.DropEvent += HandleDrop;
            _input.UseEventCancelled += HandleCancelUse;
        }

        private void SelectSlot(int numberIn, bool bypassDeselect = false)
        {
            //dont try to select if there is no item
            if (numberIn >= hotbarSlots.Length || hotbarSlots[numberIn].Item == null)
            {
                return;
            }
            //if the slot is equipped, unequip it (unless bypassed)
            if (currentSelectedSlot_ID == numberIn && !bypassDeselect)
            { 
                DeselectSlots();
                return;
            }
            //if the slot is -1 and in the hotbar
            else if (currentSelectedSlot_ID == -1 && numberIn < hotbarSlots.Length)
            {
                currentSelectedSlot_ID = numberIn;
                CurrentSelectedSlot.slotAnimator.SetBool(SelectedHash, true);
                playerArmAnimator.SetInteger("ItemID", CurrentSelectedSlot.Item.ID);
                CurrentSelectedSlot.SlotGameObject.GetComponent<Image>().color = Color.black;
                SlotAnimSelect(CurrentSelectedSlot);
                return;
            }

            //if the slot is in the hotbar and not -1
            if (numberIn < hotbarSlots.Length)
            { 
                SlotAnimDeselect(CurrentSelectedSlot);
                currentSelectedSlot_ID = numberIn;

                CurrentSelectedSlot.slotAnimator.SetBool(SelectedHash, true);
                playerArmAnimator.SetInteger("ItemID", CurrentSelectedSlot.Item.ID);
                CurrentSelectedSlot.SlotGameObject.GetComponent<Image>().color = Color.black;
                SlotAnimSelect(CurrentSelectedSlot);
            }
        }


        private void DeselectSlots()
        {
            SlotAnimDeselect(CurrentSelectedSlot);
            playerArmAnimator.SetInteger("ItemID", 0);
            currentSelectedSlot_ID = -1;
        }

        /// <summary>
        /// Uses a float from 0 to 9 and sets the selected inventory slot to that float.
        /// </summary>
        /// (Yes its 0 to 9 even though we only have 5 slots it might get used for other stuff don't worry.)
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
                        CurrentSelectedSlot.Item.TimesUsed += 1;
                        RemoveItemFromSlot(currentSelectedSlot_ID);
                    }
                }
            }
            catch (Exception) { }
        }

        public void HandleUseAnimation()
        {
            playerArmAnimator.ResetTrigger("ForceStopUse");
            playerArmAnimator.SetTrigger("Use");
        }

        public void HandleDrop()
        {
            if (currentSelectedSlot_ID != -1)
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
                playerArmAnimator.SetTrigger("ForceStopUse");
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
            for (int i = currentSelectedSlot_ID; i < hotbarSlots.Length-1; i++)
            {
                if (hotbarSlots[i + 1].Item != null)
                {
                    hotbarSlots[i].AddItem(hotbarSlots[i + 1].Item);
                    hotbarSlots[i + 1].ClearSlot();
                }
            }

            if (GetItemsInInventory() == 0 || !CurrentSelectedSlot.Item)
            {
                DeselectSlots();
            }
            else
            {
                SelectSlot(currentSelectedSlot_ID, true);
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

        IEnumerator WaitForAnimations(float _timeIn, InventorySlot _slotIn)
        {
            yield return new WaitForSeconds(_timeIn);
            _slotIn.slotAnimator.SetBool(SelectingHash, false);
            _slotIn.slotAnimator.SetBool(DeselectingHash, false);
            _slotIn._selectRoutine = null;
            _slotIn._deselectRoutine = null;
        }

        public void SlotAnimSelect(InventorySlot _slotIn)
        {
            if(_slotIn._selectRoutine != null || !CurrentSelectedSlot.Item)
            {
                return;
            }

            _slotIn.slotAnimator.SetBool(SelectingHash, true);
            _slotIn._selectRoutine = StartCoroutine(WaitForAnimations(_slotIn.slotAnimator.runtimeAnimatorController.animationClips[1].length, _slotIn));
        }

        public void SlotAnimDeselect(InventorySlot _slotIn)
        {
            if(_slotIn._deselectRoutine != null)
            {
                return;
            }

            CurrentSelectedSlot.SlotGameObject.GetComponent<Image>().color = Color.white;
            CurrentSelectedSlot.slotAnimator.SetBool(SelectedHash, false);
            _slotIn.slotAnimator.SetBool(DeselectingHash, true);
            _slotIn._deselectRoutine = StartCoroutine(WaitForAnimations(_slotIn.slotAnimator.runtimeAnimatorController.animationClips[0].length, _slotIn));
        }
	}

    [Serializable]
    public class InventorySlot
    {
        [SerializeField] public int Slot_ID;
        [SerializeField] public GameObject SlotGameObject;
        [SerializeField] public Image Slot_Image;
        [SerializeField] public InventoryItem Item;
        [SerializeField] public Animator slotAnimator;
        [HideInInspector] public Coroutine _selectRoutine;
        [HideInInspector] public Coroutine _deselectRoutine;
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
