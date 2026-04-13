using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/HotDudeEvent")]
    public class HotDudeEvent : EventClass
    {
        [SerializeField]
        private GameObject _hotDudePrefab;

        //When room spawns in
        public override bool Generate(CarriageClass room) { return true; }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room) { return true; }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room) { return true; }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
        {
            GameObject _hotDude = Instantiate(_hotDudePrefab);
            _hotDude.transform.parent = room.Holder;
            Transform _entry = room.transform.Find("Exit");
            _hotDude.transform.position = new Vector3(_entry.position.x, _entry.position.y + 0.1f, _entry.position.z - 0.5f);
            _hotDude.GetComponent<NavMeshAgent>().enabled = true;
            return true;
        }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room) { return true; }
        //First time completing room
        public override bool FirstExit(CarriageClass room)
        {
            if (!room.Holder.Find("HotDude(Clone)")) return false;
            room.Holder.Find("HotDude(Clone)").GetComponent<Monster>().DestroyMonster();
            return true;
        }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room) { return true; }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room) { return true; }
        //Getting far away from the room
        public override bool Recede(CarriageClass room) { return true; }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room) { return true; }
    }
}
