using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/SpeedSyringe")]
    public class SpeedSyringe : InventoryItem
    {
        private PlayerController _controller;
        public int duration = 10;
        public int stackDuration = 5;

        public float speedMultiplier = 1.3f;
        public float staminaMultiplier = 1.75f;
        public float endNegativeStaminaMultiplier = 0.75f;

        public override bool Use()
        {
            Debug.Log("SpeedSyringe");
            _controller = PlayerStatusEffects.Instance.gameObject.GetComponent<PlayerController>();
            _controller.StartRoutine(Run());
            return true;
        }

        IEnumerator Run()
        {
            _controller._timeRemaining = _controller._isRunning ? _controller._timeRemaining + stackDuration : duration;
            _controller.CurrentStamina = (int)(_controller.TotalStamina * staminaMultiplier);
            if (_controller._isRunning) { yield break;  }
            _controller._isRunning = true;

            float baseSpeed = _controller.BaseSpeed;
            float crouchSpeed = _controller.CrouchSpeed;
            float sprintSpeed = _controller.SprintSpeed;

            _controller.BaseSpeed = baseSpeed * speedMultiplier;
            _controller.CrouchSpeed = crouchSpeed * speedMultiplier * 1.2f;
            _controller.SprintSpeed = sprintSpeed * speedMultiplier * 0.9f;

            _controller.UpdateSpeed();

            while (_controller._timeRemaining > 0)
            {
                yield return new WaitForSeconds(1);
                _controller._timeRemaining -= 1;
            }

            _controller.CurrentStamina = -(int)(_controller.TotalStamina * endNegativeStaminaMultiplier);

            _controller.BaseSpeed = baseSpeed;
            _controller.CrouchSpeed = crouchSpeed;
            _controller.SprintSpeed = sprintSpeed;
            _controller.UpdateSpeed();
            _controller._isRunning = false;
        }
    }
}
