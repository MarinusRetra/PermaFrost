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
        public override bool Entered(CarriageClass room)
        {
            //Spawn tickets please
            GameObject _spawnedTicketsPlease = Instantiate(_ticketsPleasePrefab);
            _spawnedTicketsPlease.transform.parent = room.Holder;
            _spawnedTicketsPlease.transform.GetChild(0).GetComponent<Monster>().CurrentRoom = room.transform;

            //tp it to the start of the room
            Transform _entry = room.transform.Find("Exit");
            _spawnedTicketsPlease.transform.GetChild(0).position = new Vector3(_entry.position.x, _entry.position.y + 0.1f, _entry.position.z - 0.5f);
            _spawnedTicketsPlease.transform.GetChild(0).GetComponent<NavMeshAgent>().enabled = true;
            return true;
        }
        public override bool Exited(CarriageClass room)
        {
            PlrRefs.inst.PlayerMonsterManager.HasFoundTicket = false;
            if (room.Holder.Find("TicketsPlease(Clone)"))
            {
                room.Holder.Find("TicketsPlease(Clone)").transform.GetChild(0).GetComponent<Monster>().DestroyMonster();
            }
            return true;
        }
        public override bool Triggered(CarriageClass room) { return true; }
        public override bool Generated(CarriageClass room) 
        {
            //Spawn ticket
            List<Transform> _availableSpots = room.SpawnPoints[2].GetComponentsInChildren<Transform>().ToList();
            _availableSpots.RemoveAt(0);
            Transform _chosenSpot = _availableSpots[Random.Range(0, _availableSpots.Count)];
            GameObject _ticket = Instantiate(_ticketPrefab);
            _ticket.transform.position = _chosenSpot.position;
            _ticket.transform.rotation = _chosenSpot.rotation;
            _ticket.transform.parent = room.Holder;
            return true; 
        }
        public override bool CallForDeletion(CarriageClass room)
        {
            Destroy(room.Holder.Find("Ticket(Clone)")?.gameObject);
            return true;
        }
    }
}
