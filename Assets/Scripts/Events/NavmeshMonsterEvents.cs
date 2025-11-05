using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/AllEars")]
    public class AllEarsEvent : EventClass
    {
        [SerializeField] private GameObject _allEarsPrefab;
        private GameObject _spawnedAllEars;
        public override bool Entered(GameObject room)
        {
            _spawnedAllEars = Instantiate(_allEarsPrefab);
            _spawnedAllEars.transform.parent = room.transform.Find("Monsters");
            _spawnedAllEars.GetComponent<Monster>().CurrentRoom = room.transform;
            _spawnedAllEars.transform.localPosition = new Vector3(0, 0, 0);
            _spawnedAllEars.GetComponent<NavMeshAgent>().enabled = true;
            return true;
        }
        public override bool Exited(GameObject room)
        {
            room.transform.Find("Monsters").Find("AllEars(Clone)").GetComponent<Monster>().DestroyMonster();
            return true;
        }
        public override bool Triggered(GameObject room)
        {
            return true;
        }
    }

    [CreateAssetMenu(menuName = "Events/TicketsPlease")]
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

            _spawnedTicketsPlease.transform.GetChild(0).position = room.transform.Find("Exit").position;
            _spawnedTicketsPlease.transform.GetChild(0).GetComponent<NavMeshAgent>().enabled = true;

            Transform[] _availableSpots = room.transform.Find("TicketSpots").GetComponentsInChildren<Transform>();
            Transform _chosenSpot = _availableSpots[Random.Range(0, _availableSpots.Length)];
            GameObject _ticket = Instantiate(_ticketPrefab);
            _ticket.transform.parent = room.transform;
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
