using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/StalkerEvent")]
    public class StalkerEvent : EventClass
    {
        [SerializeField]
        private GameObject _stalkerPrefab;

        //When room spawns in
        public override bool Generate(CarriageClass room) { return true; }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room) { return true; }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room) { return true; }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
        {
            GameObject _stalker = Instantiate(_stalkerPrefab);
            _stalker.GetComponent<Monster>().CurrentRoom = room.transform;
            _stalker.transform.parent = room.Holder;
            return true;
        }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room) { return true; }
        //First time completing room
        public override bool FirstExit(CarriageClass room)
        {
            if (!room.Holder.Find("Stalker(Clone)")) { return true; }
            room.Holder.Find("Stalker(Clone)").GetComponent<Monster>().DestroyMonster();
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
