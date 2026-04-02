using UnityEngine;

namespace Gameplay
{
    public class SideroomArea : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlrRefs.inst.PlayerMonsterManager.InSideroom = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlrRefs.inst.PlayerMonsterManager.InSideroom = false;
            }
        }
    }
}
