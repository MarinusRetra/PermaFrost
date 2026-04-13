using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/HealingAltarEvent")]
    public class HealingAlltarEvent : EventClass
    {
        //Variables
        [SerializeField]
        private GameObject _altarPrefab;
        private Transform _chosenSpot;

        //When room spawns in
        public override bool Generate(CarriageClass room)
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
        //First time approaching room
        public override bool FirstApproach(CarriageClass room)
        {
            return RepeatApproach(room);
        }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room)
        {
            room.Holder.Find("HealingAltar(Clone)")?.gameObject.SetActive(true);
            return true;
        }
        //First time room entered
        public override bool FirstEnter(CarriageClass room){return true;}
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room) { return true; }
        //First time completing room
        public override bool FirstExit(CarriageClass room)
        {
            //REPLACE WITH BREAK ALTAR
            room.Holder.Find("HealingAltar(Clone)")?.GetComponent<HealingAltar>().DestroyAltar();
            return true;
        }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room) { return true; }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room) { return true; }
        //Getting far away from the room
        public override bool Recede(CarriageClass room)
        {
            room.Holder.Find("HealingAltar(Clone)")?.gameObject.SetActive(false);
            return true;
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            Destroy(room.Holder.Find("HealingAltar(Clone)")?.gameObject);
            return true;
        }
    }
}
