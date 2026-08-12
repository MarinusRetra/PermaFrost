using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class HealingAltar : InteractObject
    {
        private Coroutine _currentTine;
        [SerializeField] private Animator anima;
        [SerializeField] private MeshRenderer outlineRenderer;
        bool isActive = false;
        public override void Start()
        {
            base.Start();

            InteractEvent.AddListener(() =>
            {
                if (PlrRefs.inst.PlayerInventory.CurrentSelectedSlot?.Item.name != "Key")
                {
                    _currentTine = StartCoroutine(PlrRefs.inst.PlayerHealth.HealPlayer());
                    PlrRefs.inst.PlayerInventory.RemoveItemFromSlot(PlrRefs.inst.PlayerInventory.CurrentSelectedSlot.Slot_ID);
                }
            });
        }

        public void DestroyAltar()
        {
            //CurrentTine prevents player from becoming invulnerable by despawning the altar right after healing.
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
            //prevent any more interacting
            InteractEvent.RemoveAllListeners();
            Destroy(transform.GetChild(0).gameObject);
            Destroy(GetComponent<Collider>());
            yield return new WaitForSeconds(PlrRefs.inst.PlayerHealth.HealInvincibility + 1);
            Destroy(gameObject);
        }

        public void SetAnimator(bool vuln)
        {
            anima.SetBool("PlayerVulnerable", vuln);
            isActive = vuln;
            outlineRenderer.enabled = vuln;
        }

        public override void Interact()
        {
            if (isActive)
            {
                InteractEvent.Invoke();
            }
        }
    }
}
