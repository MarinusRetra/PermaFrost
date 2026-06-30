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
            //spawn door
            if (room.RoomEventRefs && room.RoomEventRefs.OpenDoor)
            {
                Vector3 doorPos = room.RoomEventRefs.OpenDoor.transform.position;
                spawnedDoor = Instantiate(scriptable.SpawnablePrefab, doorPos, scriptable.SpawnablePrefab.transform.rotation);
                spawnedDoor.transform.parent = room.Holder;

                room.RoomEventRefs.OpenDoor.SetActive(false);

                //spawn key
                EventMultiObjScriptable objEvent = scriptable as EventMultiObjScriptable;
                spawnedKey = Instantiate(objEvent.otherPrefabs[0], room.GetRandomItemSpot().position, Quaternion.identity);
                spawnedKey.GetComponent<ItemImportance>().OnSpawnKill();
                spawnedKey.transform.parent = room.Holder;
            }
            else { Debug.LogWarning("Coulnd spawn key event: No open door."); }
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
        public override bool FirstEnter(CarriageClass room) 
        { 
            RepeatEnter(room);
            return true; 
        }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room) 
        {
            if (!spawnedKey) { return true; }
            if (!keyRigidb)
            {
                keyRigidb = spawnedKey.GetComponent<Rigidbody>();
            }
            keyRigidb.isKinematic = false;
            if (spawnedKey && spawnedKey.transform.position.y < -10)
            {
                PlrRefs.inst.PlayerInventory.PickupItem(spawnedKey.GetComponent<ItemInteractable>()._item);
                Destroy(spawnedKey);
            }
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
            if(room.RoomEventRefs && room.RoomEventRefs.OpenDoor)
            {
                room.RoomEventRefs.OpenDoor.SetActive(true);
            }
            Destroy(this);
            return true;
        }
    }
}
