using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/MusicBoxEvent")]
    public class MusicBoxEvent : EventClass
    {
        [SerializeField]
        private GameObject _musicBoxPrefab;
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
            GameObject _box = Instantiate(_musicBoxPrefab);
            _box.transform.parent = room.Holder;
            _box.transform.localScale = new Vector3(1, 1, 1);
            _box.transform.position = _chosenSpot.position;
            _box.transform.rotation = _chosenSpot.rotation;
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
            Transform box = room.Holder.Find("MusicBox(Clone)");
            if (box == null) { return false; }
            box.gameObject.SetActive(true);
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
            Transform box = room.Holder.Find("MusicBox(Clone)");
            if (box == null) { return false; }
            box.GetComponent<MusicBox>().SetBoxIdleState(false);
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
            Transform box = room.Holder.Find("MusicBox(Clone)");
            if (box == null) { return false; }
            box.GetComponent<MusicBox>().SetBoxIdleState(true);
            PlrRefs.inst.PlayerStatusEffects.ManageInsanityCauses("Music", true);
            return true;
        }
        //Getting far away from the room
        public override bool Recede(CarriageClass room)
        {
            Transform box = room.Holder.Find("MusicBox(Clone)");
            if (box == null) { return false; }
            box.gameObject.SetActive(false);
            return true;
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            Transform box = room.Holder.Find("MusicBox(Clone)");
            if (box == null) { return false; }
            Destroy(box.gameObject);
            return true;
        }
    }
}
