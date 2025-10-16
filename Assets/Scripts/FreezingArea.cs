using UnityEngine;

namespace Gameplay
{
    public class FreezingArea : MonoBehaviour
    {
        [SerializeField] private string _freezingName;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerStatusEffects.Instance.ManageFrostbiteCauses(_freezingName,false);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerStatusEffects.Instance.ManageFrostbiteCauses(_freezingName, true);
            }
        }
    }
}
