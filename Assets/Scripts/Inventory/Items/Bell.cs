using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/Bell")]
    public class Bell : InventoryItem
    {
        [SerializeField] private int _value = -100;
        public override bool Use()
        {
            PlayerStatusEffects.Instance.AddInstantInsanity(_value);
            return true;
        }
    }
}
