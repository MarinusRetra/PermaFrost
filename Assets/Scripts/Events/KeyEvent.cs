using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class KeyEvent : EventClass
    {
        private GameObject spawnedKey;
        private GameObject spawnedDoor;
        private Rigidbody keyRigidb;

        //When room spawns in
        public override bool Generate(CarriageClass room)
        {
            //find key spot
            List<Transform> _availableSpots = room.SpawnPoints[0].GetComponentsInChildren<Transform>().ToList();
            _availableSpots.RemoveAt(0);
            Transform randomLocation = _availableSpots[Random.Range(0, _availableSpots.Count)];

            //spawn door
            Vector3 doorPos = room.ExitPoint.position + new Vector3(0, 1.4f, 0);
            spawnedDoor = Instantiate(scriptable.SpawnablePrefab, doorPos, scriptable.SpawnablePrefab.transform.rotation);
            spawnedDoor.transform.parent = room.Holder;

            //spawn key
            EventMultiObjScriptable objEvent = scriptable as EventMultiObjScriptable;
            spawnedKey = Instantiate(objEvent.otherPrefabs[0], randomLocation.position, Quaternion.identity);
            spawnedKey.GetComponent<ItemImportance>().OnSpawnKill();
            spawnedKey.transform.parent = room.Holder;
            return true;
        }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room) { return RepeatApproach(room); }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room) 
        {
            if (!spawnedKey) { return true; }
            if (!keyRigidb)
            {
                keyRigidb = spawnedKey.GetComponent<Rigidbody>();
            }
            keyRigidb.isKinematic = false;
            return true; 
        }
        //First time room entered
        public override bool FirstEnter(CarriageClass room) { return true; }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room) { return true; }
        //First time completing room
        public override bool FirstExit(CarriageClass room) { return true; }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room) { return true; }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room) { return true; }
        //Getting far away from the room
        public override bool Recede(CarriageClass room) 
        {
            if (!spawnedKey) { return true; }
            if (!keyRigidb)
            {
                keyRigidb = spawnedKey.GetComponent<Rigidbody>();
            }
            keyRigidb.isKinematic = true;
            return true; 
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            if (spawnedDoor) { Destroy(spawnedDoor); }
            if (spawnedKey) {  Destroy(spawnedKey); }
            Destroy(this);
            return true;
        }
    }
}
