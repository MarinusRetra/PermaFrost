using System;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement values")]
        [SerializeField] private InputReader _input;
        [SerializeField] private Transform _camera;
        [SerializeField] private float _crouchSpeed = 3;
        [SerializeField] private float _sprintSpeed= 7;
        [SerializeField] private float _baseSpeed = 4;
        [Header("Camera values")]
        [SerializeField] private float _sensitivity = 0.4f;
        [SerializeField] private float _crouchCameraHeight = 0f;
        [SerializeField] private float _standCameraHeight = 0.5f;
        private Rigidbody _rb;
        private float _rotationY;
        private float _cameraRotationX;
        private float _currentMoveSpeed = 4;
        private CapsuleCollider _playerCollider;
        private Vector3 _moveDirection;
        private float _moveInputX, _moveInputY;
        private bool _isHoldingCrouch = false;
        private bool _isCrouching = false;
        private bool _canGetUp = true;
        private bool _isChangingSize = false;
        private Vector2 _crouchHitboxHeight = new(1.4f, -0.308f);
        private Vector2 _standHitboxHeight = new(2, 0);

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _rb = GetComponent<Rigidbody>();
            _playerCollider = GetComponent<CapsuleCollider>();

            _input.MoveEvent += HandleMove;
            _input.CrouchEvent += HandleCrouch;
            _input.CrouchCancelEvent += HandleCrouchCancel;
            _input.SprintCancelEvent += HandleSprintCancel;
            _input.SprintEvent += HandleSprint;
            _input.LookEvent += HandleLook;
            _input.NextPreviousEvent += HandleHotbarNav;
            _input.HotbarSelectEvent += HandleHotbarSelect;
            _input.UseEvent += HandleUse;
            _input.InteractEvent += HandleInteract;
        }

        private void FixedUpdate()
        {
            Move();
            if (_isCrouching)
            {
                if (!_isHoldingCrouch && _canGetUp)
                {
                    StandUp();
                }
            }
        }

        /// <summary>
        /// Gets a vector 2 from a joystick or WASD and sets the move direction.
        /// </summary>
        private void HandleMove(Vector2 obj)
        {
            _moveInputX = obj.x;
            _moveInputY = obj.y;
        }

        /// <summary>
        /// Triggers the interact logic of the interacted item.
        /// </summary>
        private void HandleInteract()
        {

        }

        /// <summary>
        /// Use item in selected hotbar slot by triggering the item's logic.
        /// </summary>
        private void HandleUse()
        {

        }

        /// <summary>
        /// Uses a float from 0 to 9 and sets the selected inventory slot to that float.
        /// </summary>
        private void HandleHotbarSelect(float number)
        {

        }

        /// <summary>
        /// A positive or negative number used to navigate either left or right through the hotbar for scrolling and controller navigation.
        /// </summary>
        private void HandleHotbarNav(float obj)
        {

        }

        /// <summary>
        /// Uses mouse position to or joystick delta to rotate the first person camera.
        /// </summary>
        void HandleLook(Vector2 obj)
        {
            _cameraRotationX = Math.Clamp(_cameraRotationX -= obj.y * _sensitivity, -90, 90);
            _rotationY += obj.x * _sensitivity;

            transform.rotation = Quaternion.Euler(0, _rotationY, 0);
            _camera.localRotation = Quaternion.Euler(_cameraRotationX, 0, 0);
        }
        private void HandleCrouch()
        {
            _isHoldingCrouch = true;
            CrouchDown();
        }

        private void HandleCrouchCancel()
        {
            _isHoldingCrouch = false;
            if (_canGetUp)
            {
                StandUp();
            }
        }

        private void HandleSprint()
        {
            if (!_isCrouching)
            { 
                _currentMoveSpeed = _sprintSpeed;
            }
        }

        private void HandleSprintCancel()
        {
            _currentMoveSpeed = _baseSpeed;
        }


        /// <summary>
        /// Uses the move direction calculated from input to move the player in that direction.
        /// </summary>
        private void Move()
        {
            _moveDirection = transform.right * _moveInputX + transform.forward * _moveInputY;

            _rb.linearVelocity = _currentMoveSpeed * _moveDirection + new Vector3(0, _rb.linearVelocity.y, 0);
        }
        /// <summary>
        /// Crouch down by changing the height of the capsule collider and lowering the camera.
        /// </summary>
        public void CrouchDown()
        {
            if (!_isCrouching)
            {
                _currentMoveSpeed = _crouchSpeed;
                _playerCollider.height = _crouchHitboxHeight.x;
                _playerCollider.center = new(0, _crouchHitboxHeight.y, 0);
                _camera.localPosition = new Vector3(0, _crouchCameraHeight, 0);
            }
            _isCrouching = true;
        }
        public void StandUp()
        {
            _currentMoveSpeed = _baseSpeed;
            _playerCollider.height = _standHitboxHeight.x;
            _playerCollider.center = new(0, _standHitboxHeight.y, 0);
            _camera.localPosition = new Vector3(0, _standCameraHeight, 0);
            _isCrouching = false;
        }



        void OnTriggerEnter(Collider other)
        {
            _canGetUp = false;
        }
        void OnTriggerExit(Collider other)
        {
            _canGetUp = true;
        }
        // void OnTriggerStay(Collider other)
        // {
        //     _canGetUp = false;
        // }

        private void OnDestroy()
        {
            _input.SprintCancelEvent -= HandleSprintCancel;
            _input.SprintEvent -= HandleSprint;
            _input.MoveEvent -= HandleMove;
            _input.CrouchEvent -= HandleCrouch;
            _input.CrouchCancelEvent -= HandleCrouchCancel;
        }

        private void OnDisable()
        {
            _input.SprintCancelEvent -= HandleSprintCancel;
            _input.SprintEvent -= HandleSprint;
            _input.MoveEvent -= HandleMove;
            _input.CrouchEvent -= HandleCrouch;
            _input.CrouchCancelEvent -= HandleCrouchCancel;
        }
    }
}
