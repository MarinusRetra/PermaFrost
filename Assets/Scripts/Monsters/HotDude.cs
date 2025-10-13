using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    public class HotDude : Monster
    {
        private enum hotStates {Agressive,Stunned,Idle}
        private hotStates _currentState = hotStates.Agressive;

        private Transform _player;

        public Transform CurrentRoom;

        [SerializeField] private NavMeshAgent _agent;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Fetch the player
            _player = FindAnyObjectByType<PlayerHealth>().transform;

            _agent = GetComponent<NavMeshAgent>();

            StartCoroutine(HandleMovement());
        }

        private IEnumerator HandleMovement()
        {
            while (true)
            {
                switch (_currentState)
                {
                    case hotStates.Agressive:
                        //check if area reached
                        _agent.destination = _player.position;
                        yield return new WaitForSeconds(0.1f);
                        continue;
                }
            }
        }

        private void Attack()
        {
            //check area for player
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    StartCoroutine(hitCollider.GetComponent<PlayerHealth>().DamagePlayer());
                }
            }
            //do visuals or something
        }
    }
}
