using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/KeyEvent")]
    public class KeyEvent : EventClass
    {
        [SerializeField]
        private GameObject _keyPrefab;
        [SerializeField]
        private GameObject _doorPrefab;

        //When room spawns in
        public override bool Generate(CarriageClass room)
        {
            //find key spot
            List<Transform> _availableSpots = room.SpawnPoints[0].GetComponentsInChildren<Transform>().ToList();
            _availableSpots.RemoveAt(0);
            Transform randomLocation = _availableSpots[Random.Range(0, _availableSpots.Count)];

            //spawn door
            Vector3 doorPos = room.transform.Find("Exit").position + new Vector3(0, 1.4f, 0);
            GameObject door = Instantiate(_doorPrefab, doorPos, _doorPrefab.transform.rotation);
            door.transform.parent = room.Holder;

            //spawn key
            GameObject newKey = Instantiate(_keyPrefab, randomLocation.position, Quaternion.identity);
            newKey.GetComponent<ItemImportance>().OnSpawnKill();
            newKey.transform.parent = room.Holder;
            return true;
        }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room) { return true; }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room) { return true; }
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
        public override bool Recede(CarriageClass room) { return true; }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            Destroy(room.Holder.Find("Key(Clone)")?.gameObject);
            Destroy(room.Holder.Find("LockedDoor(Clone)")?.gameObject);
            return true;
        }
    }
}
