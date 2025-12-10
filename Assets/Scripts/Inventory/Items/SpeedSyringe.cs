using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/SpeedSyringe")]
    public class SpeedSyringe : InventoryItem
    {
        private PlayerController _controller;

        public override bool Use()
        {
            Debug.Log("SpeedSyringe");
            _controller = PlayerStatusEffects.Instance.gameObject.GetComponent<PlayerController>();
            _controller.StartRoutine(Run());
            return true;
        }

        IEnumerator Run()
        {
            float multiplier = 1.35f;

            float baseSpeed = _controller.BaseSpeed;
            float crouchSpeed = _controller.CrouchSpeed;
            float sprintSpeed = _controller.SprintSpeed;

            _controller.BaseSpeed = baseSpeed * multiplier * 1.2f;
            _controller.CrouchSpeed = crouchSpeed * multiplier * 1.3f;
            _controller.SprintSpeed = sprintSpeed * multiplier;

            _controller.CurrentStamina = (int)(_controller.TotalStamina * 1.75f);

            yield return new WaitForSeconds(10);

            _controller.CurrentStamina = -(int)(_controller.TotalStamina * 0.75f);

            _controller.BaseSpeed = baseSpeed;
            _controller.CrouchSpeed = crouchSpeed;
            _controller.SprintSpeed = sprintSpeed;
        }
    }
}
