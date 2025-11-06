using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/NoiseMonkey")]

    public class NoiseMonkey : InventoryItem
    {
      public override bool Use()
      {
        Debug.Log("NoiseMonkey");
        return true;
      }
    }
}
