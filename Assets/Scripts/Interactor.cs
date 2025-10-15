using System;
using UnityEngine;

namespace Gameplay
{
    public class Interactor : MonoBehaviour
    {
        [Header("Interact Values")]
        [SerializeField] private float _interactDistance = 5f;
        [SerializeField] private InputReader _input;
        private Ray ray;
        private RaycastHit hit;
        private Transform _cameraTransform;

        void Start()
        {
            _input.InteractEvent += HandleInteract;
            _input.LookEvent += PlayerRaycastFromLook;
            _cameraTransform = Camera.main.transform;
        }

        private void PlayerRaycastFromLook(Vector2 mousepos)
        {
            ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            Physics.Raycast(ray, out hit, _interactDistance);
        }

        void FixedUpdate()
        {
            hit.collider?.GetComponent<InteractObject>()?.Hover();
        }
        /// <summary>
        /// Triggers the interact logic of the interacted item.
        /// </summary>
        private void HandleInteract()
        {
            hit.collider.GetComponent<InteractObject>()?.Interact();
        }

        void OnDestroy()
        {
            _input.InteractEvent -= HandleInteract;
            _input.LookEvent -= PlayerRaycastFromLook;

        }
        void OnDisable()
        {
            _input.InteractEvent -= HandleInteract;
            _input.LookEvent -= PlayerRaycastFromLook;
        }
    }
}
