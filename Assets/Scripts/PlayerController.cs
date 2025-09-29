using System;
using UnityEngine;

namespace Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputReader _input;

        [SerializeField] private int _crouchSpeed = 3;
        [SerializeField] private int _sprintSpeed = 7;
        [SerializeField] private int _baseSpeed = 4;
        private int _currentSpeed = 4;
        private Vector2 _moveDirection;


        private void Start()
        {
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

        private void Update()
        {
            Move();
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
        /// <param name="mousePos"></param>
        void HandleLook(Vector2 mousePos)
        {

        }
        /// <summary>
        /// Gets a vector 2 from a joystick or WASD and sets the move direction.
        /// </summary>
        /// <param name="direction"></param>
        private void HandleMove(Vector2 direction)
        {
            _moveDirection = direction;
        }

        private void HandleCrouch()
        {
            _currentSpeed = _crouchSpeed;
        }

        private void HandleCrouchCancel()
        {
            _currentSpeed = _baseSpeed;
        }

        private void HandleSprint()
        { 
           _currentSpeed = _sprintSpeed;
        }

        private void HandleSprintCancel()
        {
           _currentSpeed = _baseSpeed;
        }

        /// <summary>
        /// Uses the move direction calculated from input to move the player in that direction.
        /// </summary>
        private void Move()
        {
            if (_moveDirection == Vector2.zero)
            {
                return;
            }

            transform.position += new Vector3(_moveDirection.x, 0, _moveDirection.y) * (_currentSpeed * Time.deltaTime);
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
