using System;
using System.Collections;
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
		public float CrouchSpeed = 3;
		public float BaseSpeed = 4;

        [Header("Sprint values")]
        public float SprintSpeed = 7;
        [SerializeField] private int _totalStamina = 10;
        private int _currentStamina = 10;
        private bool isSprinting = false;

        [Header("Camera values")]
        private Transform _camera;
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
        private Vector2 _crouchHitboxHeight = new(1.4f, -0.308f);
        private Vector2 _standHitboxHeight = new(2, 0);

        private void Start()
        {
            
            _camera = Camera.main.transform;
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

            StartCoroutine(Sprint());
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
        /// Uses mouse position to or joystick delta to rotate the first person camera.
        /// </summary>
        void HandleLook(Vector2 obj)
        {
            _cameraRotationX = Math.Clamp(_cameraRotationX -= obj.y * _sensitivity, -90, 90);
            _rotationY += obj.x * _sensitivity;

            transform.rotation = Quaternion.Euler(0, _rotationY, 0);
            _camera.localRotation = Quaternion.Euler(_cameraRotationX, 0, 0);
            // TODO: Make it work properly with the joystick as well.
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
                _currentMoveSpeed = SprintSpeed;
                isSprinting = true;
            }
        }

        private void HandleSprintCancel()
        {
            _currentMoveSpeed = BaseSpeed;
            isSprinting = false;
        }

        private IEnumerator Sprint()
        {
            while (true)
            {
                //stamina goes down when running
                if (isSprinting)
                {
                    _currentStamina--;
                    if (_currentStamina < 0)
                    {
                        HandleSprintCancel();
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                else if (_currentStamina < _totalStamina)
                {
                    //goes up when not running
                    yield return new WaitForSeconds(0.2f);
                    _currentStamina++;
                }
                else
                {
                    //wait when at max stamina
                    yield return new WaitForSeconds(0.1f);
                }
            }
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
                _currentMoveSpeed = CrouchSpeed;
                _playerCollider.height = _crouchHitboxHeight.x;
                _playerCollider.center = new(0, _crouchHitboxHeight.y, 0);
                _camera.localPosition = new Vector3(0, _crouchCameraHeight, 0);
            }
            _isCrouching = true;
        }
        public void StandUp()
        {
            _currentMoveSpeed = BaseSpeed;
            _playerCollider.height = _standHitboxHeight.x;
            _playerCollider.center = new(0, _standHitboxHeight.y, 0);
            _camera.localPosition = new Vector3(0, _standCameraHeight, 0);
            _isCrouching = false;
        }



        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == 9) { return; }
            _canGetUp = false;
        }
        void OnTriggerExit(Collider other)
        {
            _canGetUp = true;
        }

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

        public void StartRoutine(IEnumerator routine) => StartCoroutine(routine);
    }
}
