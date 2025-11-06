using UnityEngine;

namespace Gameplay
{
    public class HealingAltar : InteractObject
    {
        public override void Start()
        {
            InteractEvent.AddListener(() =>
            {
                //check item thing
                StartCoroutine( PlayerHealth.Instance.HealPlayer());
            });
            print("Da listener is add");
            base.Start();
        }
    }
}
