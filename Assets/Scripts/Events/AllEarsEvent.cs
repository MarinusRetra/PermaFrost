using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    public class AllEarsEvent : EventClass
    {
        private GameObject spawnedAllEars;
        private AllEars spawnedAllEarsScript;

        //When room spawns in
        public override bool Generate(CarriageClass room) { return true; }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room)
        {
            spawnedAllEars = Instantiate(scriptable.SpawnablePrefab);
            spawnedAllEars.transform.parent = room.Holder;
            spawnedAllEars.transform.localPosition = new Vector3(0, 0, 0);
            spawnedAllEars.GetComponent<NavMeshAgent>().enabled = true;

            spawnedAllEarsScript = spawnedAllEars.GetComponent<AllEars>();
            spawnedAllEarsScript.CurrentRoom = room.transform;
            spawnedAllEarsScript.Start();
            spawnedAllEarsScript.SetIdleState(true);
            return true;
        }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room)
        {
            if (!spawnedAllEars) {  return true; }
            spawnedAllEars.SetActive(true);
            spawnedAllEarsScript.Start();
            spawnedAllEarsScript.SetIdleState(true);
            return true;
        }
        //First time room entered
        public override bool FirstEnter(CarriageClass room) { return RepeatEnter(room); }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room)
        {
            if (!spawnedAllEars) { return true; }
            spawnedAllEarsScript.SetIdleState(false);
            return true;
        }
        //First time completing room
        public override bool FirstExit(CarriageClass room) { return RepeatExit(room); }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room) { return RepeatExit(room); }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room)
        {
            if (!spawnedAllEars) { return true; }
            spawnedAllEarsScript.SetIdleState(true);
            return true;
        }
        //Getting far away from the room
        public override bool Recede(CarriageClass room)
        {
            if (!spawnedAllEars) { return true; }
            spawnedAllEars.SetActive(false);
            return true;
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            if (spawnedAllEars) { spawnedAllEarsScript.DestroyMonster(); }
            Destroy(this);
            return true;
        }
    }
}
