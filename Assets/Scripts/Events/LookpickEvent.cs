using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/LookpickEvent")]
    public class LookpickEvent : EventClass
    {
        [SerializeField]
        private GameObject _lookpickPrefab;
        public override bool Entered(GameObject room)
        {
            return true;
        }
        public override bool Exited(GameObject room)
        {
            return true;
        }
        public override bool Triggered(GameObject room)
        {
            return true;
        }
    }
}
