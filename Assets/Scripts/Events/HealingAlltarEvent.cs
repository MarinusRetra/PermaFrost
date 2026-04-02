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
        public override bool Entered(CarriageClass room)
        {
            return true;
        }
        public override bool Exited(CarriageClass room)
        {
            room.Holder.Find("HealingAltar(Clone)")?.GetComponent<HealingAltar>().DestroyAltar();
            return true;
        }
        public override bool Triggered(CarriageClass room) { return true; }
        public override bool Generated(CarriageClass room) 
        {
            List<Transform> _availableSpots = room.SpawnPoints[1].GetComponentsInChildren<Transform>().ToList();
            _availableSpots.RemoveAt(0);
            _chosenSpot = _availableSpots[Random.Range(0, _availableSpots.Count)];

            //make sure it cant spawn on the same spot as box
            if (_chosenSpot.name == "CHOSENBYBOX")
            {
                _availableSpots.Remove(_chosenSpot);
                _chosenSpot = _availableSpots[Random.Range(0, _availableSpots.Count)];
            }
            _chosenSpot.name = "CHOSENBYALTAR";

            //spawn altar
            GameObject _altar = Instantiate(_altarPrefab);
            _altar.transform.parent = room.Holder;
            _altar.transform.localScale = new Vector3(1, 1, 1);
            _altar.transform.position = _chosenSpot.position;
            _altar.transform.rotation = _chosenSpot.rotation;
            return true; 
        }
        public override bool CallForDeletion(CarriageClass room)
        {
            Destroy(room.Holder.Find("HealingAltar(Clone)")?.gameObject);
            return true;
        }
    }
}
