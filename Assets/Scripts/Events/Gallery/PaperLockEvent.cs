using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Gameplay
{
    public class PaperLockEvent : EventClass
    {
        private GameObject spawnedPlate;
        private GameObject spawnedWraps;

        //When room spawns in
        public override bool Generate(CarriageClass room)
        {
            return true;
        }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room) { return RepeatApproach(room); }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room)
        {
            return true;
        }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
        {
            //find Plate spot
            List<Transform> _availableSpots = room.SpawnPoints[1].GetComponentsInChildren<Transform>(false).ToList();
            _availableSpots.RemoveAt(0);
            Transform randomLocation = _availableSpots[Random.Range(0, _availableSpots.Count)];

            //spawn wraps
            Vector3 doorPos = room.ExitPoint.transform.position;
            spawnedWraps = Instantiate(scriptable.SpawnablePrefab, doorPos + new Vector3(0,1,0), scriptable.SpawnablePrefab.transform.rotation);
            spawnedWraps.transform.parent = room.Holder;

            //spawn plate
            EventMultiObjScriptable objEvent = scriptable as EventMultiObjScriptable;
            spawnedPlate = Instantiate(objEvent.otherPrefabs[0], randomLocation.position, objEvent.otherPrefabs[0].transform.rotation);
            spawnedPlate.transform.parent = room.Holder;
            spawnedPlate.GetComponent<StandinInPlate>().WrapBlockage = spawnedWraps.GetComponent<PaperWrapBlockage>();
            return true;
        }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room)
        {
            return true;
        }
        //First time completing room
        public override bool FirstExit(CarriageClass room) { return true; }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room) { return true; }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room) { return true; }
        //Getting far away from the room
        public override bool Recede(CarriageClass room)
        {
            if (!spawnedPlate) { return true; }
            return true;
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            if (spawnedWraps) { Destroy(spawnedWraps); }
            if (spawnedPlate) { Destroy(spawnedPlate); }
            Destroy(this);
            return true;
        }
    }
}
