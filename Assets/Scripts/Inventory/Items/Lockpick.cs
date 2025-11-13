using System.Collections;
using Controls;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/Lockpick")]
    public class Lockpick : InventoryItem
    {
        private bool _cancelledLockpicking = false;
        private float _lockPickingTime = 0;
        private float _lockPickFinishTime = 4;
        private GameObject lookinAt;
        private PlayerController _controller;

        public override bool Use()
        {
            _cancelledLockpicking = false;
            _controller = PlayerStatusEffects.Instance.gameObject.GetComponent<PlayerController>();
            lookinAt = Camera.main.GetComponent<Interactor>().hit.collider?.gameObject;
            if (lookinAt && lookinAt.CompareTag("Door"))
            {
                _controller._input.UseEventCancelled += OnLeftClickRelease;
                _controller.StartCoroutine(Lockpicking());
            }
            return false;
        }
        IEnumerator Lockpicking()
        {
            while (!_cancelledLockpicking)
            {
                yield return new WaitForSeconds(0.1f);
                _lockPickingTime += 0.1f;
                if (_lockPickingTime >= _lockPickFinishTime)
                {
                    Destroy(lookinAt);
                    break;
                }
            }
        }
        private void OnLeftClickRelease()
        {
            _lockPickingTime = 0;
            _cancelledLockpicking = true;
        }
        void OnDestroy()
        {
            _controller._input.UseEventCancelled -= OnLeftClickRelease;

        }
        void OnDisable()
        {
            _controller._input.UseEventCancelled -= OnLeftClickRelease;
        }
    }
}
