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
        public override bool Entered(CarriageClass room)
        {
            return true;
        }
        public override bool Exited(CarriageClass room) { return true; }
        public override bool Triggered(CarriageClass room) { return true; }

        public override bool Generated(CarriageClass room) 
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
        public override bool CallForDeletion(CarriageClass room)
        {
            Destroy(room.Holder.Find("Key(Clone)")?.gameObject);
            Destroy(room.Holder.Find("LockedDoor(Clone)")?.gameObject);
            return true;
        }
    }
}
