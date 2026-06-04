using UnityEngine;

namespace Gameplay
{
    public class StalkerEvent : EventClass
    {
        private GameObject[] spawnedStalkers = new GameObject[3];
        private Stalker[] spawnedStalkerClass = new Stalker[3];

        //When room spawns in
        public override bool Generate(CarriageClass room) { return true; }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room) { return true; }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room) { return true; }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject spawnedStalker = Instantiate(scriptable.SpawnablePrefab);
                spawnedStalkerClass[i] = spawnedStalker.GetComponent<Stalker>();
                spawnedStalkerClass[i].CurrentRoom = room.transform;
                spawnedStalkerClass[i].CurrentCarriage = room;
                spawnedStalker.transform.parent = room.Holder;
                spawnedStalkers[i] = spawnedStalker;
            }
            return true;
        }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room) { return true; }
        //First time completing room
        public override bool FirstExit(CarriageClass room)
        {
            for (int i = 0; i < 3; i++)
            spawnedStalkerClass[i].DestroyMonster();
            return true;
        }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room) { return true; }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room) { return true; }
        //Getting far away from the room
        public override bool Recede(CarriageClass room) { return true; }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room) 
        {
            Destroy(this);
            return true; 
        }
    }
}
