using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/HeatPack")]
    public class HeatPack : InventoryItem
    {
        [SerializeField] private int _value = -70;
        public override bool Use()
        {
            PlayerStatusEffects.Instance.AddInstantFrostbite(_value);
            Debug.Log("Heatpack");
            return true;
        }
    }
}
