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
                //HD would block the door is stunned.
                objec.GetComponent<HotDude>().DestroyMonster();
            }
            if (objec.CompareTag("Player"))
            {
                PlrRefs.inst.PlayerStatusEffects.ManageFrostbiteCauses(_freezingName, state);
            }
        }
    }
}
