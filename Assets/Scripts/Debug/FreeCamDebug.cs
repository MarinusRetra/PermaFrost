using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Gameplay
{
    #if DEBUG
    public class FreeCamDebug : MonoBehaviour
    {
        GameObject player;
        PlayerHealth playerHealth;
        PlayerStatusEffects playerEffects;
        Rigidbody rb;
        bool inFreeCam = false;

        bool seperatedCam = false;

        [SerializeField] private InputReader _input;
        private float _currentMoveSpeed = 4;
        private Vector3 _moveDirection;
        private float _moveInputX, _moveInputY;

        private float _cameraRotationY;
        private float _cameraRotationX;
        [SerializeField] private float _sensitivity = 0.4f;
        private Transform _camera;

        private void OnEnable()
        {
            _input.MoveEvent += HandleMove;
            _input.LookEvent += HandleLook;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = PlrRefs.inst.gameObject;
            _camera = player.transform.GetChild(0);
            playerHealth = PlrRefs.inst.PlayerHealth;
            playerEffects = PlrRefs.inst.PlayerStatusEffects;
            rb = player.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.fKey.wasPressedThisFrame)
            {
                player.GetComponent<Rigidbody>().useGravity = false;
                player.GetComponent<Collider>().enabled = false;
                playerHealth.HealInvincibility = 9999999;
                playerHealth.DamagePlayer("Skill issue");
                playerHealth.HealPlayer(true);
                playerEffects.InsanityDeath = 9999;
                playerEffects.FrostbiteDeath = 9999;
                inFreeCam = true;
                playerHealth.CheckPlayerUnderMap = false;
                _input.CrouchEvent -= PlrRefs.inst.PlayerController.HandleCrouch;
                GameObject.Find("UI").GetComponent<Canvas>().enabled = false;
            }

            if (keyboard.mKey.wasPressedThisFrame && inFreeCam)
            {
                if (!seperatedCam)
                {
                    PlrRefs.inst.PlayerController.enabled = false;
                    seperatedCam = true;
                }
                else
                {
                    PlrRefs.inst.PlayerController.enabled = true;
                    _input.CrouchEvent -= PlrRefs.inst.PlayerController.HandleCrouch;
                    seperatedCam = false;
                }
            }
            if (inFreeCam)
            {
                if (keyboard.numpadPlusKey.wasPressedThisFrame)
                {
                    _currentMoveSpeed++;
                }
                if (keyboard.numpadMinusKey.wasPressedThisFrame)
                {
                    _currentMoveSpeed--;
                }
            }
        }

        private void FixedUpdate()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (seperatedCam)
            {
                Move();
            }
            if (inFreeCam)
            {
                if (keyboard.spaceKey.isPressed)
                {
                    player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.1f, player.transform.position.z);
                }
                if (keyboard.ctrlKey.isPressed)
                {
                    player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 0.1f, player.transform.position.z);
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

        private void Move()
        {
            if (seperatedCam)
            {
                _moveDirection = transform.right * _moveInputX + transform.forward * _moveInputY;

                rb.linearVelocity = _currentMoveSpeed * _moveDirection + new Vector3(0, rb.linearVelocity.y, 0);
            }
        }

        /// <summary>
        /// Uses mouse position to or joystick delta to rotate the first person camera.
        /// </summary>
        void HandleLook(Vector2 obj)
        {
            if (seperatedCam)
            {
                _cameraRotationX = Math.Clamp(_cameraRotationX -= obj.y * _sensitivity, -90, 90);
                _cameraRotationY = _cameraRotationY += obj.x * _sensitivity;

                _camera.localRotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY, 0);
            }
        }
    }
    #endif
}
