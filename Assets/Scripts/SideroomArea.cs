using UnityEngine;

namespace Gameplay
{
    public class SideroomArea : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerMonsterManager.Instance.InSideroom = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerMonsterManager.Instance.InSideroom = false;
            }
        }
    }
}
