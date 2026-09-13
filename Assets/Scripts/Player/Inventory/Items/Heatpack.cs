using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/HeatPack")]
    public class HeatPack : InventoryItem
    {
        [SerializeField] private int time = 3;
        public override bool Use()
        {
            if(PlrRefs.inst.PlayerStatusEffects._currentFrostbite == 0) { return false; }

            bool usedPack = PlrRefs.inst.PlayerStatusEffects.AddOvertimeHeat("Pack", time, false);
            if (!usedPack) { return false; }

            PlrRefs.inst.PlayerInventory.HandleUseAnimation();
            return true;
        }
    }
}
