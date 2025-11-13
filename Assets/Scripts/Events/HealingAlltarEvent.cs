using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/HealingAltarEvent")]
    public class HealingAlltarEvent : EventClass
    {
        [SerializeField]
        private GameObject _altarPrefab;
        private Transform _chosenSpot;
        public override bool Entered(GameObject room)
        {
            List<Transform> _availableSpots = room.transform.Find("MusicBoxSpots").GetComponentsInChildren<Transform>().ToList();
            _availableSpots.RemoveAt(0);
            _chosenSpot = _availableSpots[Random.Range(0, _availableSpots.Count)];
            if(_chosenSpot.name == "CHOSENBYBOX")
            {
                _availableSpots.Remove(_chosenSpot);
                _chosenSpot = _availableSpots[Random.Range(0, _availableSpots.Count)];
            }
            _chosenSpot.name = "CHOSENBYALTAR";
            GameObject _altar = Instantiate(_altarPrefab);
            _altar.transform.parent = room.transform.Find("BoxHolder");
            _altar.transform.localScale = new Vector3(1, 1, 1);
            _altar.transform.position = _chosenSpot.position;
            _altar.transform.rotation = _chosenSpot.rotation;
            return true;
        }
        public override bool Exited(GameObject room)
        {
            Destroy(room.transform.Find("BoxHolder").Find("HealingAltar(Clone)").gameObject);
            return true;
        }
        public override bool Triggered(GameObject room)
        {
            return true;
        }
    }
}
