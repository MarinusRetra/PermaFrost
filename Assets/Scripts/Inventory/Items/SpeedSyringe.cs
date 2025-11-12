using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/SpeedSyringe")]
    public class SpeedSyringe : InventoryItem
    {
        PlayerController controller;

        public override bool Use()
        {
            Debug.Log("SpeedSyringe");
            controller = PlayerStatusEffects.Instance.gameObject.GetComponent<PlayerController>();
            controller.StartRoutine(Run());
            return true;
        }

        IEnumerator Run()
        {
            float multiplier = 1.35f;

            float baseSpeed = controller.BaseSpeed;
            float crouchSpeed = controller.CrouchSpeed;
            float sprintSpeed = controller.SprintSpeed;

            controller.BaseSpeed = baseSpeed * multiplier * 1.2f;
            controller.CrouchSpeed = crouchSpeed * multiplier * 1.3f;
            controller.SprintSpeed = sprintSpeed * multiplier;

            controller.CurrentStamina = (int)(controller.TotalStamina * 1.75f);

            yield return new WaitForSeconds(10);

            controller.CurrentStamina = -(int)(controller.TotalStamina * 0.75f);

            controller.BaseSpeed = baseSpeed;
            controller.CrouchSpeed = crouchSpeed;
            controller.SprintSpeed = sprintSpeed;
        }
    }
}
