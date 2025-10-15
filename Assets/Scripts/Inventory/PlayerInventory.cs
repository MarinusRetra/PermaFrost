using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] InventoryItem EmptyItem;
        [SerializeField] GameObject _HotbarObject;
        private List<RectTransform> _hotbarUIElements;
        [SerializeField] private List<InventoryItem> _hotbar;
        private InventoryItem _selectedHotbarItem;
        [SerializeField] InputReader _input;

        void Awake()
        {
            _input.HotbarSelectEvent += HandleHotbarSelect;
            _input.NextPreviousEvent += HandleHotbarNav;
            _input.UseEvent += HandleUse;
        }

        void Start()
        {
            _hotbarUIElements = _HotbarObject.GetComponentsInChildren<RectTransform>().ToList();

            IEnumerable<RectTransform> query = _hotbarUIElements.Where(_HotbarElement => _HotbarElement.CompareTag("HUD"));
            _hotbarUIElements = query.Cast<RectTransform>().ToList();

            _selectedHotbarItem = _hotbar[0];
            UpdateSlots();
        }

        /// <summary>
        /// Uses a float from 0 to 9 and sets the selected inventory slot to that float.
        /// </summary>
        private void HandleHotbarSelect(float number)
        {
            if (number > _hotbar.Count-1)
            {
                return;
            }
            foreach (var element in _hotbarUIElements) //TODO: vervang dit door iets dat het laatste geselecteerde element normaal zet.
            {
                element.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            }
            _selectedHotbarItem = _hotbar[(int)number];

            _hotbarUIElements[(int)number].transform.localScale = new(0.6f, 0.6f, 1.2f);

            RemoveEmptySlots();
        }

        /// <summary>
        /// A positive or negative number used to navigate either left or right through the hotbar for scrolling and controller navigation.
        /// </summary>
        private void HandleHotbarNav(float obj)
        {
            //selectedHotbarItem = _hotbar.ElementAt(_hotbar.IndexOf(selectedHotbarItem) + (int)obj);
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
            if (_selectedHotbarItem != EmptyItem && _hotbar.Count > 0)
            {
                _selectedHotbarItem?.Use();
                _selectedHotbarItem = EmptyItem;
                RemoveEmptySlots();
            }
        }

        public void DropItem()
        {
            _hotbar.Remove(_selectedHotbarItem);
            // Instantiate item
        }
        /// <summary>
        /// Clears out all the slots with an EmptyItem inside.
        /// </summary>
        void RemoveEmptySlots()
        {
            for (var i = 0; i < _hotbarUIElements.Count; i++)
            {
                if (_hotbar[i] == EmptyItem)
                {
                    _hotbar.RemoveAt(i);
                    _hotbarUIElements[i].gameObject.SetActive(false);
                    _hotbarUIElements.RemoveAt(i);
                }
                UpdateSlots();
            }
        }

        /// <summary>
        /// Will set the sprite and color of every slot to sprite and color of the contained item.
        /// </summary>
        void UpdateSlots()
        {
            //Zet de _hotbarUIElements gelijk met _hotbar en zet de images van de hotbar.
            for (var i = 0; i < _hotbarUIElements.Count; i++)
            {
                var currentImage = _hotbarUIElements[i].gameObject.GetComponentInChildren<Image>();
                if (_hotbar[i] != EmptyItem)
                {
                    currentImage.sprite = _hotbar[i].sprite;
                    currentImage.color = _hotbar[i].color;
                }
                else
                {
                    currentImage.sprite = EmptyItem.sprite;
                    currentImage.color = EmptyItem.color;
                }
            }
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
