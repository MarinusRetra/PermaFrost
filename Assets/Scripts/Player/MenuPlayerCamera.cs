using UnityEngine;

namespace Gameplay
{
    public class MenuPlayerCamera : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        public float _sensitivity = 0.4f;
        private float _rotationY;
        private float _cameraRotationX;
        [SerializeField] private Transform _camera;

        private void OnEnable()
        {
            _input.LookEvent += HandleLook;
            _input.PauseEvent += HandlePause;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        /// <summary>
        /// Uses mouse position to or joystick delta to rotate the first person camera.
        /// </summary>
        void HandleLook(Vector2 obj)
        {
            _cameraRotationX = Mathf.Clamp(_cameraRotationX -= obj.y * (_sensitivity * 0.1f), -15f, 15f);
            _rotationY = Mathf.Clamp(_rotationY += obj.x * (_sensitivity * 0.1f), -15f, 15f);

            _camera.localRotation = Quaternion.Euler(_cameraRotationX, (_rotationY), 0);
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
        }

        private void OnDisable()
        {
            _input.LookEvent -= HandleLook;
            _input.PauseEvent -= HandlePause;
        }
    }
}
