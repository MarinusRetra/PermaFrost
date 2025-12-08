using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class HealingAltar : InteractObject
    {
        private Coroutine _currentTine;
        public override void Start()
        {
            InteractEvent.AddListener(() =>
            {
                if (PlayerHealth.Instance.gameObject.GetComponent<PlayerInventory>().RemoveSelectedSlot())
                {
                    _currentTine = StartCoroutine(PlayerHealth.Instance.HealPlayer());
                }
            });
            base.Start();
        }

        public void DestroyAltar()
        {
            if(_currentTine != null)
            {
                StartCoroutine(WaitForDestroy());
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator WaitForDestroy()
        {
            InteractEvent.RemoveAllListeners();
            Destroy(transform.GetChild(0).gameObject);
            Destroy(GetComponent<Collider>());
            yield return new WaitForSeconds(PlayerHealth.Instance.HealInvincibility + 1);
            Destroy(gameObject);
        }
    }
}
