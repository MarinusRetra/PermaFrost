using System;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody _rb;
        [Header("Movement values")]
        [SerializeField] private InputReader _input;
        [SerializeField] private Transform _camera;
        [SerializeField] private float _crouchSpeedMultiplier = 3;
        [SerializeField] private float _sprintSpeedMultiplier = 7;
        [SerializeField] private float _baseSpeed = 4;
        [SerializeField] private float _sensitivity = 0.5f;
        private float _rotationY;
        private float _cameraRotationX;
        private float _currentMoveSpeed = 4;
        private Vector3 _moveDirection;
        private float _moveInputX, _moveInputY;
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _rb = GetComponent<Rigidbody>();

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
        }

        /// <summary>
        /// Gets a vector 2 from a joystick or WASD and sets the move direction.
        /// </summary>
        /// <param name="obj"></param>
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
        /// <param name="number"></param>
        private void HandleHotbarSelect(float number)
        {

        }

        /// <summary>
        /// A positive or negative number used to navigate either left or right through the hotbar for scrolling and controller navigation.
        /// </summary>
        /// <param name="obj"></param>
        private void HandleHotbarNav(float obj)
        {

        }

        /// <summary>
        /// Uses mouse position to or joystick delta to rotate the first person camera.
        /// </summary>
        /// <param name="obj"></param>
        void HandleLook(Vector2 obj)
        {
            _cameraRotationX = Math.Clamp(_cameraRotationX -= obj.y * _sensitivity, -90, 90);
            _rotationY += obj.x * _sensitivity;

            transform.rotation = Quaternion.Euler(0, _rotationY, 0);
            _camera.localRotation = Quaternion.Euler(_cameraRotationX, 0, 0);
        }
        private void HandleCrouch()
        {
            _currentMoveSpeed *= _crouchSpeedMultiplier;
        }

        private void HandleCrouchCancel()
        {
            _currentMoveSpeed = _baseSpeed;
        }

        private void HandleSprint()
        { 
           _currentMoveSpeed *= _sprintSpeedMultiplier;
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
