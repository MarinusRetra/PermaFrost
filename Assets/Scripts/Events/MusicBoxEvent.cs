using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace Gameplay
{
    public class MusicBoxEvent : EventClass
    {
        private GameObject spawnedBox;
        private MusicBox spawnedBoxClass;
        private Transform _chosenSpot;

        //When room spawns in
        public override bool Generate(CarriageClass room)
        {
            List<Transform> _availableSpots = room.SpawnPoints[1].GetComponentsInChildren<Transform>().ToList();
            _availableSpots.RemoveAt(0);
            _chosenSpot = _availableSpots[Random.Range(0, _availableSpots.Count)];

            //Make sure altar and box cant spawn on the same spot
            if (_chosenSpot.name == "CHOSENBYALTAR")
            {
                _availableSpots.Remove(_chosenSpot);
                _chosenSpot = _availableSpots[Random.Range(0, _availableSpots.Count)];
            }
            _chosenSpot.name = "CHOSENBYBOX";

            //Spawn box
            GameObject _box = Instantiate(scriptable.SpawnablePrefab);
            _box.transform.parent = room.Holder;
            _box.transform.localScale = new Vector3(1, 1, 1);
            _box.transform.position = _chosenSpot.position;
            _box.transform.rotation = _chosenSpot.rotation;
            spawnedBox = _box;
            spawnedBoxClass = _box.GetComponent<MusicBox>();

            //EventAnimScriptable animEvent = scriptable as EventAnimScriptable;
            //print(animEvent.printThisMan);
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
            spawnedBox.SetActive(true);
            return true;
        }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
        {
            return RepeatEnter(room);
        }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room)
        {
            spawnedBoxClass.SetBoxIdleState(false);
            return true;
        }
        //First time completing room
        public override bool FirstExit(CarriageClass room)
        {
            return RepeatExit(room);
        }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room)
        {
            return RepeatExit(room);
        }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room)
        {
            spawnedBoxClass.SetBoxIdleState(true);
            PlrRefs.inst.PlayerStatusEffects.ManageInsanityCauses("Music", true);
            return true;
        }
        //Getting far away from the room
        public override bool Recede(CarriageClass room)
        {
            spawnedBox.SetActive(false);
            return true;
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            if (spawnedBox)
            {
                Destroy(spawnedBox);
            }
            Destroy(this);
            return true;
        }
    }
}
