using UnityEngine;

namespace Gameplay
{
    public class FreezingArea : MonoBehaviour
    {
        [SerializeField] private string _freezingName;

        private void OnTriggerEnter(Collider other)
        {
            AffectObject(other.gameObject, false);
        }

        private void OnTriggerExit(Collider other)
        {
            AffectObject(other.gameObject, true);
        }

        public void AffectObject(GameObject objec,bool state)
        {
            if (objec.GetComponent<HotDude>())
            {
                objec.GetComponent<HotDude>().DestroyMonster();
            }
            if (objec.CompareTag("Player"))
            {
                PlayerStatusEffects.Instance.ManageFrostbiteCauses(_freezingName, state);
            }
        }
    }
}
