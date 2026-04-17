using System;
using UnityEngine;

namespace Gameplay
{
    public class Interactor : MonoBehaviour
    {
        [Header("Interact Values")]
        [SerializeField] private float _interactDistance = 5f;
        [SerializeField] private InputReader _input;
        private Ray _ray;
        public RaycastHit hit;
        private Transform _cameraTransform;
        public LayerMask playerLayer;

        private InteractObject _cachedCollision;
        private InteractObject _lastCachedCollision;

        public void Start()
        {
            
            _input.LookEvent += HandleMouseMovement;
            _input.InteractEvent += HandleInteract;
            _cameraTransform = PlrRefs.inst.Camera.transform;
        }
        /// <summary>
        /// Triggers the interact logic of the interacted item.
        /// </summary>
        private void HandleInteract()
        {
            if (_cachedCollision != null && _cachedCollision.enabled)
            {
                _cachedCollision.Interact();
            }
        }

        /// <summary>
        /// Triggers the hover logic of interactObject you are looking at.
        /// </summary>
        private void HandleMouseMovement(Vector2 _numIn)
        {
            _ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            Physics.Raycast(_ray, out hit, _interactDistance, ~playerLayer);
            _cachedCollision = hit.collider ? hit.collider.GetComponent<InteractObject>() : null;
            if(_cachedCollision != null)
            {
                _lastCachedCollision = _cachedCollision;
                _cachedCollision.IsHovered = true;
                _cachedCollision.Hover();
            }
            else if(_lastCachedCollision != null)
            {
                _lastCachedCollision.IsHovered = false;
                _lastCachedCollision.Hover();
            }
        }

        void OnDestroy()
        {
            _input.InteractEvent -= HandleInteract;
            _input.LookEvent -= HandleMouseMovement;
        }
        void OnDisable()
        {
            _input.InteractEvent -= HandleInteract;
            _input.LookEvent -= HandleMouseMovement;
        }
    }
}
