using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/HeatPack")]
    public class HeatPack : InventoryItem
    {
        [SerializeField] private int value = -70;
        public override bool Use()
        {
            PlayerStatusEffects.Instance.AddInstantFrostbite(value);
            Debug.Log("Heatpack");
            return true;
        }
    }
}
