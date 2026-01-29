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


        private void OnEnable()
        {
            _input.MoveEvent += HandleMove;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = GameObject.Find("Player");
            playerHealth = player.GetComponent<PlayerHealth>();
            playerEffects = player.GetComponent<PlayerStatusEffects>();
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
            }

            if (inFreeCam)
            {
                if (keyboard.numpad8Key.isPressed)
                {
                    player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.1f, player.transform.position.z);
                }
                if (keyboard.numpad2Key.isPressed)
                {
                    player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 0.1f, player.transform.position.z);
                }
            }

            if (keyboard.mKey.wasPressedThisFrame && inFreeCam)
            {
                if (!seperatedCam)
                {
                    player.GetComponent<PlayerController>().enabled = false;
                    seperatedCam = true;
                }
                else
                {
                    player.GetComponent<PlayerController>().enabled = true;
                    seperatedCam = false;
                }

                if (inFreeCam)
                {
                    if (keyboard.numpadPlusKey.wasPressedThisFrame)
                    {
                        _currentMoveSpeed++;
                        print("Speed up");
                    }
                    if (keyboard.numpadMinusKey.wasPressedThisFrame)
                    {
                        _currentMoveSpeed--;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (seperatedCam)
            {
                Move();
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
            _moveDirection = transform.right * _moveInputX + transform.forward * _moveInputY;

            rb.linearVelocity = _currentMoveSpeed * _moveDirection + new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
    #endif
}
