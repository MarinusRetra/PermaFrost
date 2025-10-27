using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    public class TicketsPlease : Monster
    {
        private NavMeshAgent _agent;
        [SerializeField] private GameObject _hitbox;
        private Transform _exitRoom;
        private Transform _entryRoom;

        private bool _isChasing = false;
        private bool _reachedEnd = false;
        private void Start()
        {
            //open exit door (idk how)
            //spawn ticket/tickets (up to ryan)
            _agent = GetComponent<NavMeshAgent>();
            _exitRoom = CurrentRoom.Find("exit");
            _entryRoom = CurrentRoom.Find("entry");
            transform.position = _exitRoom.position;
            _agent.destination = new Vector3(_entryRoom.position.x,transform.position.y,_entryRoom.position.z);
        }

        private void Update()
        {
            if (_agent.remainingDistance < 0.1f && !_isChasing)
            {
                _agent.destination = new Vector3(_entryRoom.position.x, transform.position.y, _entryRoom.position.z - 20);
                if (!_reachedEnd)
                {
                    StartCoroutine(ReachedEnd());
                }
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!PlayerMonsterManager.Instance.HasFoundTicket && !_isChasing)
                {
                    StartCoroutine(ChasePlayer());
                }
            }
        }

        public override void Deaggro()
        {
            _isChasing = false;
            _agent.speed = 1;
            _agent.destination = new Vector3(_entryRoom.position.x, transform.position.y, _entryRoom.position.z);
        }

        private IEnumerator ChasePlayer()
        {
            _isChasing = true;
            //VERY fast, so player cant just run past and despawn them by going into the next room.
            _agent.speed = 8;
            while (!PlayerMonsterManager.Instance.HasFoundTicket && _isChasing)
            {
                _agent.destination = PlayerMonsterManager.Instance.transform.position;
                yield return new WaitForSeconds(0.2f);
            }
            Deaggro();
        }

        private IEnumerator ReachedEnd()
        {
            //look I would make this all fancy and stuff but we do not have time for that at all.
            _reachedEnd = true;
            yield return new WaitForSeconds(10f);
            Destroy(gameObject);
        }
    }
}
