using UnityEngine;

namespace Gameplay
{
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
}
