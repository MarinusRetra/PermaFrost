using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    public class TicketsPleaseEvent : EventClass
    {
        //Variables
        private GameObject spawnedTicketsPlease;
        private GameObject spawnedTicket;

        //When room spawns in
        public override bool Generate(CarriageClass room)
        {
            if(room.SpawnPoints.Length < 2) { Debug.LogWarning("No ticket spots found. Event not continuing."); return true; }
            //Spawn ticket
            List<MeshFilter> _availableSpots = room.SpawnPoints[2].GetComponentsInChildren<MeshFilter>(false).ToList();
            _availableSpots.RemoveAt(0);
            MeshFilter _chosenSpot = _availableSpots[Random.Range(0, _availableSpots.Count)];
            EventMultiObjScriptable objEvent = scriptable as EventMultiObjScriptable;
            GameObject _ticket = Instantiate(objEvent.otherPrefabs[0]);
            _ticket.transform.position = _chosenSpot.transform.position;
            _ticket.transform.rotation = _chosenSpot.transform.rotation;
            _ticket.transform.parent = room.InstanceHolder;
            spawnedTicket = _ticket;
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
            if (spawnedTicket) { spawnedTicket.SetActive(true); }
            return true;
        }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
        {
            if (!spawnedTicket) { return false; }
            //Spawn tickets please
            GameObject _spawnedTicketsPlease = Instantiate(scriptable.SpawnablePrefab);
            GameObject mainTPObj = _spawnedTicketsPlease.transform.GetChild(0).gameObject;
            _spawnedTicketsPlease.transform.parent = room.InstanceHolder;
            Monster ticketsMonster = mainTPObj.GetComponent<Monster>();
            ticketsMonster.CurrentRoom = room.transform;
            ticketsMonster.CurrentCarriage = room;

            //tp it to the start of the room
            Transform _entry = room.transform.Find("Exit");
            mainTPObj.transform.position = new Vector3(_entry.position.x, _entry.position.y + 0.1f, _entry.position.z - 0.5f);
            mainTPObj.GetComponent<NavMeshAgent>().enabled = true;
            spawnedTicketsPlease = _spawnedTicketsPlease;
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
            if (spawnedTicket) { spawnedTicket.SetActive(false); }
            return true;
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            if (spawnedTicket) { Destroy(spawnedTicket); }
            if (spawnedTicketsPlease) {  Destroy(spawnedTicketsPlease); }
            Destroy(this);
            return true;
        }
    }
}
