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
        public override bool Entered(GameObject room)
        {
            return true;
        }
        public override bool Exited(GameObject room) { return true; }
        public override bool Triggered(GameObject room) { return true; }

        public override bool Generated(GameObject room) 
        {
            //find key spot
            List<Transform> _availableSpots = room.transform.Find("SpawnPoints").GetComponentsInChildren<Transform>().ToList();
            _availableSpots.RemoveAt(0);
            Transform randomLocation = _availableSpots[Random.Range(0, _availableSpots.Count)];

            //spawn door
            Vector3 doorPos = room.transform.Find("Exit").position + new Vector3(0, 1.4f, 0);
            GameObject door = Instantiate(_doorPrefab, doorPos, Quaternion.identity);

            //spawn key
            GameObject newKey = Instantiate(_keyPrefab, randomLocation.position, Quaternion.identity);
            newKey.GetComponent<ItemImportance>().OnSpawnKill();
            return true; 
        }
    }
}
