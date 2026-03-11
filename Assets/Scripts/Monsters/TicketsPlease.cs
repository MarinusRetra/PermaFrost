using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    public class TicketsPlease : Monster
    {
        private NavMeshAgent _agent;
        [SerializeField] private GameObject _hitbox;
        private Transform _entryRoom;

        private bool _isChasing = false;
        private bool _reachedEnd = false;

        private Vector3 _currentDestination;
        private void Start()
        {
            _entryRoom = CurrentRoom.Find("Entry");

            _agent = GetComponent<NavMeshAgent>();
            _agent.destination = new Vector3(_entryRoom.position.x,transform.position.y,_entryRoom.position.z);
            _currentDestination = _agent.destination;
        }

        private void Update()
        {
            //Check if it has reached the end
            if (_agent.isOnNavMesh && _agent?.remainingDistance < 0.1f && !_isChasing)
            {
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
            //force it to stop chasing
            _isChasing = false;
            _agent.speed = 0.75f;
            _agent.destination = _currentDestination;
        }

        private IEnumerator ChasePlayer()
        {
            _isChasing = true;
            //VERY fast, so player cant just run past and despawn them by going into the next room. (they still can most of the time)
            _agent.speed = 8;
            while (!PlayerMonsterManager.Instance.HasFoundTicket && _isChasing && transform.position.z < _entryRoom.position.z + 30)
            {
                _agent.destination = PlayerMonsterManager.Instance.transform.position;
                yield return new WaitForSeconds(0.2f);
            }
            Deaggro();
        }

        private IEnumerator ReachedEnd()
        {
            //Make ticketsplease explode 10 seconds after it gets to the exit. During this time its destination is set 20 away, so it should continue walking if there is enough space
            _agent.destination = new Vector3(_entryRoom.position.x, transform.position.y, _entryRoom.position.z - 20);
            _currentDestination = _agent.destination;
            _reachedEnd = true;
            yield return new WaitForSeconds(20f);
            DestroyMonster();
        }

        public override void DestroyMonster()
        {
            transform.parent.gameObject.name = "Despawning";
            _agent.enabled = false;
            ParticleSystem parti = transform.Find("Particle System").GetComponent<ParticleSystem>();
            parti.gameObject.SetActive(true);
            parti.Play();
            parti.transform.parent = null;
            Destroy(parti.gameObject,parti.main.duration);
            Destroy(gameObject, parti.main.duration / 2);
        }
    }
}
