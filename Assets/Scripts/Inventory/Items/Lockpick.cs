using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/Lockpick")]
    public class Lockpick : InventoryItem
    {
        [SerializeField] private float _lockPickFinishTime = 4f;
        [SerializeField] private InputReader _input;
        [SerializeField] private GameObject _lookPickPrefab;
        private GameObject _currentLookPickInstance;
        private GameObject lookinAt;

        public override bool Use()
        {
            lookinAt = Camera.main.GetComponent<Interactor>().hit.collider?.gameObject;
            if (lookinAt && lookinAt.CompareTag("Door"))
            {
                StartTimer(_lockPickFinishTime);
                _currentLookPickInstance = Instantiate(_lookPickPrefab);
                _input.CanModifyHotbar = false;
            }
            return false;
        }
        public override void UseCancelled()
        {
            base.UseCancelled();
            Destroy(_currentLookPickInstance);
            _input.CanModifyHotbar = true;
        }

        public override void CompleteTimer()
        {
            Destroy(lookinAt);
            Destroy(_currentLookPickInstance);
            Camera.main.GetComponentInParent<PlayerInventory>().CurrentSelectedSlot.ClearSlot();
            _input.CanModifyHotbar = true;
        }
    }
}
