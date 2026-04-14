using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class HealingAlltarEvent : EventClass
    {
        //Variables
        private Transform _chosenSpot;
        private GameObject spawnedAltar;

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
            GameObject _altar = Instantiate(scriptable.SpawnablePrefab);
            _altar.transform.parent = room.Holder;
            _altar.transform.localScale = new Vector3(1, 1, 1);
            _altar.transform.position = _chosenSpot.position;
            _altar.transform.rotation = _chosenSpot.rotation;
            spawnedAltar = _altar;

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
            if (spawnedAltar)
            {
                spawnedAltar.SetActive(true);
            }
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
            spawnedAltar?.GetComponent<HealingAltar>().DestroyAltar();
            return true;
        }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room) { return true; }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room) { return true; }
        //Getting far away from the room
        public override bool Recede(CarriageClass room)
        {
            if (spawnedAltar)
            {
                spawnedAltar.SetActive(false);
            }
            return true;
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            if (spawnedAltar) { Destroy(spawnedAltar); }
            Destroy(this);
            return true;
        }
    }
}
