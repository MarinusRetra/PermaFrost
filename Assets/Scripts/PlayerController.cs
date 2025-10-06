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
        }


        private void Update()
        {
            Move();
        }

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

        private void Move() // Deze hele functie is tijdlijk geen zorgen.
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
