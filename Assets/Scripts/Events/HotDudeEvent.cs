using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/HotDudeEvent")]
    public class HotDudeEvent : EventClass
    {
        [SerializeField]
        private GameObject _hotDudePrefab;
        public override bool Entered(CarriageClass room)
        {
            GameObject _hotDude = Instantiate(_hotDudePrefab);
            _hotDude.transform.parent = room.Holder;
            Transform _entry = room.transform.Find("Exit");
            _hotDude.transform.position = new Vector3(_entry.position.x, _entry.position.y + 0.1f, _entry.position.z - 0.5f);
            _hotDude.GetComponent<NavMeshAgent>().enabled = true;
            return true;
        }
        public override bool Exited(CarriageClass room)
        {
            if (!room.Holder.Find("HotDude(Clone)")) return false;
            room.Holder.Find("HotDude(Clone)").GetComponent<Monster>().DestroyMonster();
            return true;
        }
        public override bool Triggered(CarriageClass room) { return true; }

        public override bool Generated(CarriageClass room) { return true; }
        public override bool CallForDeletion(CarriageClass room) { return true; }
    }
}
