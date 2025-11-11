using UnityEngine;

namespace Gameplay
{
    public class HealingAltar : InteractObject
    {
        public override void Start()
        {
            InteractEvent.AddListener(() =>
            {
                if (PlayerHealth.Instance.gameObject.GetComponent<PlayerInventory>().RemoveSelectedSlot())
                {
                    StartCoroutine(PlayerHealth.Instance.HealPlayer());
                }
            });
            base.Start();
        }
    }
}
