using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/HeatPack")]
    public class HeatPack : InventoryItem
    {
        [SerializeField] private int _value = -70;
        public override bool Use()
        {
            PlrRefs.inst.PlayerStatusEffects.AddInstantFrostbite(_value);
            return true;
        }
    }
}
