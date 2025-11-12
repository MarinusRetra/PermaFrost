using System.Collections;
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
        float multiplier = 1.5f;

        float baseSpeed = controller.BaseSpeed;
        float crouchSpeed = controller.CrouchSpeed;
        float sprintSpeed = controller.SprintSpeed;

        controller.BaseSpeed = baseSpeed * multiplier;
        controller.CrouchSpeed = crouchSpeed * multiplier;
        controller.SprintSpeed = sprintSpeed * multiplier;

        yield return new WaitForSeconds(10);

        controller.BaseSpeed = baseSpeed;
        controller.CrouchSpeed = crouchSpeed;
        controller.SprintSpeed = sprintSpeed;
    }
}
}
