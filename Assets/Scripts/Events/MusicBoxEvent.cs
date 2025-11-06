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
        public override bool Entered(GameObject room)
        {
            List<Transform> _availableSpots = room.transform.Find("MusicBoxSpots").GetComponentsInChildren<Transform>().ToList();
            _availableSpots.RemoveAt(0);
            Transform _chosenSpot = _availableSpots[Random.Range(0, _availableSpots.Count)];
            GameObject _ticket = Instantiate(_musicBoxPrefab);
            _ticket.transform.parent = room.transform;
            _ticket.transform.position = _chosenSpot.position;
            _ticket.transform.rotation = _chosenSpot.rotation;
            return true;
        }
        public override bool Exited(GameObject room)
        {
            Destroy(room.transform.Find("MusicBox(Clone)").gameObject);
            return true;
        }
        public override bool Triggered(GameObject room)
        {
            return true;
        }
    }
}
