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
        //Variables

        //When room spawns in
        public override bool Generate(CarriageClass room)
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
        //First time approaching room
        public override bool FirstApproach(CarriageClass room)
        {
            return RepeatApproach(room);
        }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room)
        {
            room.Holder.Find("Ticket(Clone)")?.gameObject.SetActive(true);
            return true;
        }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
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
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room)
        {
            return true;
        }
        //First time completing room
        public override bool FirstExit(CarriageClass room)
        {
            PlrRefs.inst.PlayerMonsterManager.HasFoundTicket = false;
            return true;
        }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room)
        {
            return true;
        }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room)
        {
            return true;
        }
        //Getting far away from the room
        public override bool Recede(CarriageClass room)
        {
            room.Holder.Find("Ticket(Clone)")?.gameObject.SetActive(false);
            return true;
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            Destroy(room.Holder.Find("Ticket(Clone)")?.gameObject);
            return true;
        }
    }
}
