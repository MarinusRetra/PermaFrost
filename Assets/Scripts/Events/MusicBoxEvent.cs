using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/MusicBoxEvent")]
    public class MusicBoxEvent : EventClass
    {
        [SerializeField]
        private GameObject _musicBoxPrefab;
        private Transform _chosenSpot;
        public override bool Entered(CarriageClass room)
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
        public override bool Exited(CarriageClass room)
        {
            Transform box = room.Holder.Find("MusicBox(Clone)");
            if (box == null) { return false; }
            box.GetComponent<MusicBox>().SilenceBox(false);
            Destroy(box.gameObject);
            PlayerStatusEffects.Instance.ManageInsanityCauses("Music", true);
            return true;
        }
        public override bool Triggered(CarriageClass room) { return true; }

        public override bool Generated(CarriageClass room) { return true; }
        public override bool CallForDeletion(CarriageClass room) { return true; }
    }
}
