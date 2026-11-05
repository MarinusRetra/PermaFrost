using UnityEngine;

namespace Gameplay
{
    public class MenuPlayerCamera : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        public float _sensitivity = 0.4f;
        private float _rotationY;
        private float _cameraRotationX;
        private float _lookInputX, _lookInputY;
        [SerializeField] private Transform _camera;

        private void OnEnable()
        {
            _input.LookEvent += HandleLook;
            _input.PauseEvent += HandlePause;
            _input.InteractEvent += HandleInteract;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            _cameraRotationX = Mathf.Clamp(_cameraRotationX -= _lookInputY * (_sensitivity * 0.15f), -15f, 15f);
            _rotationY = Mathf.Clamp(_rotationY += _lookInputX * (_sensitivity * 0.15f), -15f, 35f);

            _camera.localRotation = Quaternion.Euler(_cameraRotationX, (_rotationY), 0);
        }
        /// <summary>
        /// Uses mouse position to or joystick delta to rotate the first person camera.
        /// </summary>
        void HandleLook(Vector2 obj)
        {
            _lookInputX = obj.x;
            _lookInputY = obj.y;
        }

        void HandleInteract()
        {
            print("Did Fire");
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit " + hit.collider.name);
            }
        }

        void HandlePause()
        {
            //pausing in main menu would disable the camera, this should fix that
            _input.SetGameplayActions();
        }

        private void OnDestroy()
        {
            _input.LookEvent -= HandleLook;
            _input.PauseEvent -= HandlePause;
            _input.InteractEvent -= HandleInteract;
        }

        private void OnDisable()
        {
            _input.LookEvent -= HandleLook;
            _input.PauseEvent -= HandlePause;
            _input.InteractEvent -= HandleInteract;
        }
    }
}
