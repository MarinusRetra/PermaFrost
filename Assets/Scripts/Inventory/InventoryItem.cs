using Gameplay;
using System;
using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Item/Item")]
[Serializable]
public class InventoryItem : ScriptableObject
{
    public Sprite sprite = null;
    public Color color = new(1, 1, 1, 255);
    public GameObject HoldObject = null;
    /// <summary>
    /// If the functions returns true that means the use was succesfull and it will be taken out of the hotbar. 
    /// </summary>
    /// <returns></returns>
    public virtual bool Use() { return false; }
}
[CreateAssetMenu(menuName = "Item/HeatPack")]

public class HeatPack : InventoryItem
{
    [SerializeField] private int value = -20;
    public override bool Use()
    {
        PlayerStatusEffects.Instance.AddInstantFrostbite(value);
        Debug.Log("Heatpack");
        return true;
    }
}
[CreateAssetMenu(menuName = "Item/NoiseMonkey")]

public class NoiseMonkey : InventoryItem
{
    public override bool Use()
    {
        Debug.Log("NoiseMonkey");
        return true;
    }
}
// [CreateAssetMenu(menuName = "Item/SpeedSyringe")]

// public class SpeedSyringe : InventoryItem
// {
//     PlayerController controller;

//     public override bool Use()
//     {
//         Debug.Log("SpeedSyringe");
//         controller = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
//         controller.StartRoutine(Run());
//         return true;
//     }

//     IEnumerator Run()
//     {
//         float multiplier = 1.5f;

//         float baseSpeed = controller.BaseSpeed;
//         float crouchSpeed = controller.CrouchSpeed;
//         float sprintSpeed = controller.SprintSpeed;

//         controller.BaseSpeed = baseSpeed * multiplier;
//         controller.CrouchSpeed = crouchSpeed * multiplier;
//         controller.SprintSpeed = sprintSpeed * multiplier;

//         yield return new WaitForSeconds(10);

//         controller.BaseSpeed = baseSpeed;
//         controller.CrouchSpeed = crouchSpeed;
//         controller.SprintSpeed = sprintSpeed;
//     }
// }
[CreateAssetMenu(menuName = "Item/Key")]
public class Key : InventoryItem
{
    public override bool Use()
    {
        Debug.Log("Key");
        return true;
    }
}
[CreateAssetMenu(menuName = "Item/Bell")]
public class Bell : InventoryItem
{
    [SerializeField] private int value = -20;
    public override bool Use()
    {
        PlayerStatusEffects.Instance.AddInstantInsanity(value);
        Debug.Log("Bell");
        return true;
    }
}
[CreateAssetMenu(menuName = "Item/Lockpick")]
public class Lockpick : InventoryItem
{
    public override bool Use()
    {
        Debug.Log("Lockpick");
        return true;
    }
}




