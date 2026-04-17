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
        public int TotalStamina = 10;
        public int CurrentStamina = 10;
        private bool isSprinting = false;

        [Header("Camera values")]
        private Transform _camera;
        public float _sensitivity = 0.4f;
        [SerializeField] private float _crouchCameraHeight = 0f;
        [SerializeField] private float _standCameraHeight = 0.5f;
        private Rigidbody _rb;
        private float _rotationY;
        private float _cameraRotationX;
        private float _currentMoveSpeed = 4;
        private CapsuleCollider _playerCollider;
        private Vector3 _moveDirection;
        private float _moveInputX, _moveInputY;

        //Crouch
        private bool _isHoldingCrouch = false;
        private bool _isCrouching = false;
        private bool _canGetUp = true;
        private Vector2 _crouchHitboxHeight = new(1.4f, -0.308f);
        private Vector2 _standHitboxHeight = new(2, 0);
        private Coroutine _currentCrouchRoutine;

        [Header("Sprinting")]
        private float _staminaTimer;
        [SerializeField] private float _staminaTickRate = 0.05f;
        public SprintBehaviour SprintBehav;

        [Header("SpeedSyringe")]
        public int _timeRemaining = 0;
        public bool _isRunning = false;

        [Header("Room")]
        public GameObject CurrentRoom;
        //To prevent spamming the make noise function.
        private float _noiseTimer;

        //Add all events to input
        private void OnEnable()
        {
            _input.MoveEvent += HandleMove;
            _input.CrouchEvent += HandleCrouch;
            _input.CrouchCancelEvent += HandleCrouchCancel;
            _input.SprintCancelEvent += HandleSprintCancel;
            _input.SprintEvent += HandleSprint;
            _input.LookEvent += HandleLook;
        }

        public void Start()
        {
            _camera = PlrRefs.inst.PlayerCamera.transform;
            SprintBehav.MaxSprint(TotalStamina);
            
            //auto lock mouse
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _rb = GetComponent<Rigidbody>();
            _playerCollider = GetComponent<CapsuleCollider>();
        }

        private void FixedUpdate()
        {
            Move();
            if (_isCrouching)
            {
                if (!_isHoldingCrouch && _canGetUp)
                {
                    StartCrouchRoutine(StandUp());
                }
            }
            else
            {
                if (_moveInputX != 0f || _moveInputY != 0f)
                {
                    _noiseTimer -= Time.fixedDeltaTime;
                    
                    if (_noiseTimer <= 0f)
                    {
                        PlayerMonsterManager.MakeNoise();
                        _noiseTimer = 0.2f;
                    }
                }
            }
        }

        private void Update()
        {
            HandleStamina();
            _moveDirection = transform.right * _moveInputX + transform.forward * _moveInputY;
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
            _cameraRotationX = Mathf.Clamp(_cameraRotationX -= obj.y * _sensitivity, -90f, 90f);
            _rotationY += obj.x * _sensitivity;

            transform.rotation = Quaternion.Euler(0, _rotationY, 0);
            _camera.localRotation = Quaternion.Euler(_cameraRotationX, 0, 0);
            // TODO: Make it work properly with the joystick as well.
        }
        public void HandleCrouch()
        {
            if (_isCrouching) return;
            _isHoldingCrouch = true;
            StartCrouchRoutine(CrouchDown());
        }

        private void HandleCrouchCancel()
        {
            if (!_isCrouching) return;
            _isHoldingCrouch = false;
            if (_canGetUp)
            {
                StartCrouchRoutine(StandUp());
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

        public void HandleSprintCancel()
        {
            _currentMoveSpeed = _isCrouching ? CrouchSpeed : BaseSpeed;
            isSprinting = false;
        }

        private void HandleStamina()
        {
            _staminaTimer += Time.deltaTime;

            if (_staminaTimer < _staminaTickRate) return;
            _staminaTimer = 0f;

            bool isMoving = _moveInputX != 0f || _moveInputY != 0f;

            if (isSprinting && isMoving)
            {
                CurrentStamina = Mathf.Max(0, CurrentStamina - 2);

                if (CurrentStamina == 0)
                {
                    HandleSprintCancel();
                }
            }
            else if (CurrentStamina < TotalStamina)
            {
                CurrentStamina++;
            }

            SprintBehav.UpdateSprintBar(CurrentStamina);
        }


        /// <summary>
        /// Uses the move direction calculated from input to move the player in that direction.
        /// </summary>
        private void Move()
        {

            Vector3 velocity = _rb.linearVelocity;
            velocity.x = _currentMoveSpeed * _moveDirection.x;
            velocity.z = _currentMoveSpeed * _moveDirection.z;

            _rb.linearVelocity = velocity;
        }

        /// <summary>
        /// Crouch down by changing the height of the capsule collider and lowering the camera.
         /// </summary>
        private IEnumerator CrouchDown()
        {
            if (_isCrouching) yield break;

            _currentMoveSpeed = CrouchSpeed;
            _isCrouching = true;

            float time = 0f;
            float duration = 0.25f;

            float startHeight = _playerCollider.height;
            Vector3 startCenter = _playerCollider.center;
            Vector3 startCamPos = _camera.localPosition;

            while (time < duration)
            {
                float t = time / duration;
                t = Mathf.SmoothStep(0f, 1f, t);

                _playerCollider.height = Mathf.Lerp(startHeight, _crouchHitboxHeight.x, t);
                _playerCollider.center = Vector3.Lerp(startCenter, new Vector3(0f, _crouchHitboxHeight.y, 0f), t);
                _camera.localPosition  = Vector3.Lerp(startCamPos, new Vector3(0f, _crouchCameraHeight, 0f), t);

                time += Time.deltaTime;
                yield return null; // Makes it wait until next frame to continue the loop.

                if(!_isHoldingCrouch)
                {
                    _currentCrouchRoutine = StartCoroutine(StandUp());
                    yield break;
                }
            }
            _currentCrouchRoutine = null;
        }

        private IEnumerator StandUp()
        {
            if (!_isCrouching) yield break;

            _currentMoveSpeed = BaseSpeed;
            _isCrouching = false;

            float time = 0f;
            float duration = 0.25f;

            float startHeight = _playerCollider.height;
            Vector3 startCenter = _playerCollider.center;
            Vector3 startCamPos = _camera.localPosition;

            while (time < duration)
            {
                float t = time / duration;
                t = Mathf.SmoothStep(0f, 1f, t);
                
                _playerCollider.height = Mathf.Lerp(startHeight, _standHitboxHeight.x, t);
                _playerCollider.center = Vector3.Lerp(startCenter, new Vector3(0f, _standHitboxHeight.y, 0f), t);
                _camera.localPosition  = Vector3.Lerp(startCamPos, new Vector3(0f, _standCameraHeight, 0f), t);

                time += Time.deltaTime;
                yield return null; 

                if(_isHoldingCrouch)
                {
                    _currentCrouchRoutine = StartCoroutine(CrouchDown());
                    yield break;
                }
            }
            _currentCrouchRoutine = null;
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
            _input.LookEvent -= HandleLook;
        }

        private void OnDisable()
        {
            _input.SprintCancelEvent -= HandleSprintCancel;
            _input.SprintEvent -= HandleSprint;
            _input.MoveEvent -= HandleMove;
            _input.CrouchEvent -= HandleCrouch;
            _input.CrouchCancelEvent -= HandleCrouchCancel;
            _input.LookEvent -= HandleLook;
        }

        public void StartRoutine(IEnumerator routine) => StartCoroutine(routine);

        public void UpdateSpeed()
        {
            if (_isCrouching){ _currentMoveSpeed = CrouchSpeed; return; }
            if ( isSprinting ){ _currentMoveSpeed = SprintSpeed; return; }
            _currentMoveSpeed = BaseSpeed;
        }

        private void StartCrouchRoutine(IEnumerator routine)
        {
            if (_currentCrouchRoutine != null)
            {
                StopCoroutine(_currentCrouchRoutine);
            }
        
            _currentCrouchRoutine = StartCoroutine(routine);
        }
    }
}
