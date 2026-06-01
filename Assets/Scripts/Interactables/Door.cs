using UnityEngine;

namespace Gameplay
{
    public class Door : MonoBehaviour
    {
        public InventoryItem key;
        PlayerInventory _playerInventory;
        [SerializeField] private GameObject interactable;
        [SerializeField] private Animator doorAnimator;
        private void Start()
        {
            _playerInventory = PlrRefs.inst.PlayerInventory;
        }

        public void CheckAndUseKey()
        {
            if(_playerInventory.CurrentSelectedSlot.Item == key)
            {
                _playerInventory.HandleUse();
            }
        }

        public void OpenDoor()
        {
            interactable.SetActive(false);
            doorAnimator.enabled = true;
        }
    }
}
