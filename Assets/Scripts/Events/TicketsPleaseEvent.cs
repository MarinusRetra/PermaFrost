using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/TicketsPleaseEvent")]
    public class TicketsPleaseEvent : EventClass
    {
        [SerializeField]
        private GameObject _ticketsPleasePrefab;
        [SerializeField]
        private GameObject _ticketPrefab;
        public override bool Entered(GameObject room)
        {
            GameObject _spawnedTicketsPlease = Instantiate(_ticketsPleasePrefab);
            _spawnedTicketsPlease.transform.parent = room.transform.Find("Monsters");
            _spawnedTicketsPlease.transform.GetChild(0).GetComponent<Monster>().CurrentRoom = room.transform;

            Transform _entry = room.transform.Find("Exit");
            _spawnedTicketsPlease.transform.GetChild(0).position = new Vector3(_entry.position.x, _entry.position.y + 0.1f, _entry.position.z - 0.5f);
            _spawnedTicketsPlease.transform.GetChild(0).GetComponent<NavMeshAgent>().enabled = true;

            List<Transform> _availableSpots = room.transform.Find("TicketSpots").GetComponentsInChildren<Transform>().ToList();
            _availableSpots.RemoveAt(0);
            Transform _chosenSpot = _availableSpots[Random.Range(0, _availableSpots.Count)];
            GameObject _ticket = Instantiate(_ticketPrefab);
            _ticket.transform.parent = _chosenSpot.transform;
            _ticket.transform.position = _chosenSpot.position;
            _ticket.transform.rotation = _chosenSpot.rotation;
            return true;
        }
        public override bool Exited(GameObject room)
        {
            PlayerMonsterManager.Instance.HasFoundTicket = false;
            room.transform.Find("Monsters").Find("TicketsPlease(Clone)").transform.GetChild(0).GetComponent<Monster>().DestroyMonster();
            return true;
        }
        public override bool Triggered(GameObject room)
        {
            return true;
        }
    }
}
