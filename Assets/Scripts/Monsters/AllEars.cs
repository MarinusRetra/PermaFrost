using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    public class AllEars : Monster
    {
        private enum earStates {Wandering,Agressive,Idle}
        private earStates _currentState = earStates.Wandering;

        private Transform _player;

        [Header("Not allowed radius")]
        [SerializeField] private float _playerRadius;
        [SerializeField] private float _wallRadius;
        [SerializeField] private float _lastPosRadius;
        [SerializeField] private Vector2 _lastPosition;

        [SerializeField] private Vector2[] _roomCorners;
        [SerializeField] private NavMeshAgent _agent;

        [SerializeField] private Vector2 _randomTimeBetweenMoves = new Vector2(3,6);

        private bool _despawning = false;
        private bool _spawning = true;

        [SerializeField] private AudioClip _aggroSound;
        [SerializeField] private AudioClip _spawnSound;

        void Start()
        {
            //Fetch the player
            _player = PlrRefs.inst.transform;

            _agent = GetComponent<NavMeshAgent>();

            BoxCollider renderer = CurrentRoom.GetComponent<BoxCollider>();

            //Get the rooms corners

            _roomCorners = new Vector2[]{ new Vector2(
                renderer.bounds.max.x,
                renderer.bounds.max.z),
                new Vector2(
                    renderer.bounds.min.x,
                    renderer.bounds.min.z)};

            GameObject noise = Soundsystem.PlaySound(_spawnSound, transform.position);
            noise.GetComponent<AudioSource>().volume = 0.5f;

            StartCoroutine(HandleMovement());
        }

        private IEnumerator HandleMovement()
        {
            //Spawning "animation"
            Transform model = transform.GetChild(0);
            while(model.localPosition.y < -2 && !_despawning)
            {
                model.localPosition = new Vector3(model.localPosition.x, model.localPosition.y + 0.2f, model.localPosition.z);
                yield return new WaitForSeconds(0.05f);
            }

            GetComponent<Collider>().enabled = true;
            _spawning = false;

            while (!_despawning)
            {
                switch (_currentState)
                {
                    //Keep walking around the current room
                    case earStates.Wandering:
                        Vector3 goal = GetRandomRoomPosition();
                        _agent.destination = goal;
                        yield return new WaitForSeconds(Random.Range(_randomTimeBetweenMoves.x, _randomTimeBetweenMoves.y));
                        continue;
                    //Run straight towards its destination
                    case earStates.Agressive:
                        if (_agent.remainingDistance < 0.1f)
                        {
                            _agent.isStopped = true;
                            Attack();
                            yield return new WaitForSeconds(3);
                            _agent.isStopped = false;
                            if(_agent.remainingDistance < 1f)
                            {
                                Deaggro();
                            }
                        }
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
                if (hitCollider.CompareTag("Player") && hitCollider.GetComponent<PlayerHealth>())
                {
                    StartCoroutine(hitCollider.GetComponent<PlayerHealth>().DamagePlayer("AllEars"));
                }
            }
        }

        public Vector3 GetRandomRoomPosition()
        {
            for(int i = 0; i < 15;  i++)
            {
                if(i == 14)
                {
                    print("ALLEARS FAILED TO PATHFIND. THERE IS NO FALLBACK.");
                    return Vector3.zero;
                }

                float _ranX = Random.Range(_roomCorners[0].x, _roomCorners[1].x);
                float _ranY = Random.Range(_roomCorners[0].y, _roomCorners[1].y);

                if (CheckIfAroundArea(_ranX,_player.position.x,_playerRadius) && CheckIfAroundArea(_ranY, _player.position.z, _playerRadius))
                {
                    print("AllEars tried to pathfind into the player");
                    continue;
                }

                if (_lastPosition != Vector2.zero)
                {
                    if (CheckIfAroundArea(_ranX, _lastPosition.x, _lastPosRadius) && CheckIfAroundArea(_ranY, _lastPosition.y, _lastPosRadius))
                    {
                        print("AllEars tried to pathfind into its last position!");
                        continue;
                    }
                }

                _lastPosition = new Vector2(_ranX, _ranY);
                i = 15;
            }

            return new Vector3(_lastPosition.x, transform.position.y, _lastPosition.y);
        }

        private bool CheckIfAroundArea(float value, float area, float range)
        {
            if (value < area + range && value > area - range) return true;
            return false;
        }

        public override void Deaggro()
        {
            _currentState = earStates.Wandering;
            _agent.speed = 1;
        }

        private bool _canPlaySound = true;
        public override void Aggro(Vector3 location)
        {
            //It cant be aggrod to things outside its room
            if (location.z < _roomCorners[0].y && location.z < _roomCorners[1].y || location.z > _roomCorners[0].y && location.z > _roomCorners[1].y || _spawning) return;
            
            //go to whatever called the aggro at a fast speed
            _currentState = earStates.Agressive;
            _agent.destination = new Vector3(location.x, 1, location.z);
            _agent.speed = 8;

            if (_canPlaySound)
            {
                Soundsystem.PlaySound(_aggroSound, transform.position, false, true, 0.1f);
                StartCoroutine(SoundCooldown());
            }
        }

        //Some things spam Aggro, so this is to prevent that
        public IEnumerator SoundCooldown()
        {
            _canPlaySound = false;
            yield return new WaitForSeconds(2);
            _canPlaySound = true;
        }

        public override void DestroyMonster()
        {
            gameObject.name = "Despawning";
            _despawning = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(DespawnAnim());
        }

        private IEnumerator DespawnAnim()
        {
            Transform model = transform.GetChild(0);
            while (model.localPosition.y > -7)
            {
                model.localPosition = new Vector3(model.localPosition.x, model.localPosition.y - 0.2f, model.localPosition.z);
                yield return new WaitForSeconds(0.05f);
            }
            Destroy(gameObject);
        }
    }
}
