using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    public class HotDudeEvent : EventClass
    {
        private GameObject spawnedHotDude;
        //When room spawns in
        public override bool Generate(CarriageClass room) { return true; }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room) { return true; }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room) { return true; }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
        {
            spawnedHotDude = Instantiate(scriptable.SpawnablePrefab);
            spawnedHotDude.transform.parent = room.Holder;
            Transform _entry = room.transform.Find("Exit");
            spawnedHotDude.transform.position = new Vector3(_entry.position.x, _entry.position.y + 0.1f, _entry.position.z - 0.5f);
            spawnedHotDude.GetComponent<NavMeshAgent>().enabled = true;
            return true;
        }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room) { return true; }
        //First time completing room
        public override bool FirstExit(CarriageClass room)
        {
            if (spawnedHotDude) { Destroy(spawnedHotDude); }
            return true;
        }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room) { return true; }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room) { return true; }
        //Getting far away from the room
        public override bool Recede(CarriageClass room) { return true; }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room) {
            if (spawnedHotDude) { Destroy(spawnedHotDude); }
            Destroy(this);
            return true; 
        }
    }
}
