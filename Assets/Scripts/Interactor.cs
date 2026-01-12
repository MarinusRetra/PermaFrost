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

        public void Start()
        {
            _input.InteractEvent += HandleInteract;
            _cameraTransform = Camera.main.transform;
        }

        void FixedUpdate()
        {
            _ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            Physics.Raycast(_ray, out hit, _interactDistance);
            hit.collider?.GetComponent<InteractObject>()?.Hover();
        }
        /// <summary>
        /// Triggers the interact logic of the interacted item.
        /// </summary>
        private void HandleInteract()
        {
            if (hit.collider.GetComponent<InteractObject>() && hit.collider.GetComponent<InteractObject>().enabled)
            {
                hit.collider.GetComponent<InteractObject>()?.Interact();
            }

        }

        void OnDestroy()
        {
            _input.InteractEvent -= HandleInteract;

        }
        void OnDisable()
        {
            _input.InteractEvent -= HandleInteract;
        }
    }
}
