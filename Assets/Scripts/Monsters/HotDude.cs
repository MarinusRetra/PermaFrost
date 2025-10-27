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

        [SerializeField] private NavMeshAgent _agent;

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
                        //Only way to stop em is by freezing em.
                        _agent.destination = _player.position;
                        yield return new WaitForSeconds(0.1f);
                        continue;
                    case hotStates.Idle:
                        yield return new WaitForSeconds(5);
                        _currentState = hotStates.Agressive;
                        continue;
                    default:
                        yield return new WaitForSeconds(0.5f);
                        continue;
                }
            }
        }

        private bool _isStunning = false;
        public void Stun()
        {
            if (!_isStunning)
            {
                _isStunning = true;
                StartCoroutine(StunWTime());
            }
        }
        private IEnumerator StunWTime()
        {
            _agent.speed = 1;
            bool _stoppedStun = false;

            //dont stun instantly
            for(int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(0.2f);

                //double check that there is still a lantern in the area
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, FreezingLantern._range);
                bool _stillOn = false;
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.GetComponent<FreezingLantern>())
                    {
                        _stillOn = true;
                    }
                }
                if (!_stillOn)
                {
                    yield return null;
                    _stoppedStun = true;
                    i = 10;
                }
            }
            if (!_stoppedStun)
            {
                //stun
                bool doneFreezing = false;
                _currentState = hotStates.Stunned;
                _agent.isStopped = true;
                while (!doneFreezing)
                {
                    yield return new WaitForSeconds(0.2f);

                    //while the lantern is still on, stay stunned
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2);
                    bool _continueFreeze = false;
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.GetComponent<FreezingLantern>() && hitCollider.GetComponent<FreezingLantern>().LanternOn)
                        {
                            _continueFreeze = true;
                        }
                    }
                    if (!_continueFreeze)
                    {
                        yield return new WaitForSeconds(9);
                        doneFreezing = true;
                    }
                }
            }

            //unstun
            _agent.isStopped = false;
            _currentState = hotStates.Agressive;
            _agent.speed = 3;
            _isStunning = false;

        }

        public override void Deaggro()
        {
            _currentState = hotStates.Idle;
        }
    }
}
