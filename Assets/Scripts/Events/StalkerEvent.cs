using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/StalkerEvent")]
    public class StalkerEvent : EventClass
    {
        [SerializeField]
        private GameObject _stalkerPrefab;
        public override bool Entered(CarriageClass room)
        {
            GameObject _stalker = Instantiate(_stalkerPrefab);
            _stalker.GetComponent<Monster>().CurrentRoom = room.transform;
            _stalker.transform.parent = room.Holder;
            return true;
        }
        public override bool Exited(CarriageClass room)
        {
            if (!room.Holder.Find("Stalker(Clone)")) { return true; }
            room.Holder.Find("Stalker(Clone)").GetComponent<Monster>().DestroyMonster();
            return true;
        }
        public override bool Triggered(CarriageClass room) { return true; }
        public override bool Generated(CarriageClass room) { return true; }
        public override bool CallForDeletion(CarriageClass room) { return true; }
    }
}
