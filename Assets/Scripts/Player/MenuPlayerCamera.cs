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
        }
        /// <summary>
        /// Uses mouse position to or joystick delta to rotate the first person camera.
        /// </summary>
        void HandleLook(Vector2 obj)
        {
            _cameraRotationX = Mathf.Clamp(_cameraRotationX -= obj.y * (_sensitivity * 0.1f), -25f, 25f);
            _rotationY = Mathf.Clamp(_rotationY += obj.x * (_sensitivity * 0.1f), -30f, 30f);

            _camera.localRotation = Quaternion.Euler(_cameraRotationX, (_rotationY), 0);
        }

        private void OnDestroy()
        {
            _input.LookEvent -= HandleLook;
        }

        private void OnDisable()
        {
            _input.LookEvent -= HandleLook;
        }
    }
}
