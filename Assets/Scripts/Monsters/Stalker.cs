using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Stalker : Monster
    {
        private enum StalkerStates { Watching,Watched,Moving,Idle}
        private StalkerStates _currentState = StalkerStates.Watching;

        [Header("Not allowed radius")]
        [SerializeField] private float _playerRadius;
        [SerializeField] private float _wallRadius;
        [SerializeField] private float _lastPosRadius;
        [SerializeField] private Vector3 _lastPosition;

        [SerializeField] private Vector3[] _roomCorners;

        private Transform _player;

        [Header("Sounds")]
        [SerializeField] private AudioClip _appearSound;
        [SerializeField] private AudioClip _disappearSound;

        private bool _despawning = false;

        private int _moveRNG = 0;

        void Start()
        {
            _player = PlrRefs.inst.transform;
            _moveRNG = Random.Range(-75, 25);

            BoxCollider renderer = CurrentRoom.GetComponent<BoxCollider>();

            _roomCorners = new Vector3[]{ new Vector3(
                renderer.bounds.max.x- _wallRadius,
                renderer.bounds.max.y - _wallRadius,
                renderer.bounds.max.z- _wallRadius),
                new Vector3(
                    renderer.bounds.min.x + _wallRadius,
                    renderer.bounds.min.y + 2,
                    renderer.bounds.min.z+ _wallRadius)};

            transform.position = GetRandomRoomPosition();
            Soundsystem.PlaySound(_appearSound, transform.position);
            _currentState = StalkerStates.Watching;
            StartCoroutine(HandleBehaviour());
        }

        private int _amountOfTimesNot = 0;
        private IEnumerator HandleBehaviour()
        {
            //Spawning visual
            Transform model = transform.GetChild(0);
            while (model.localPosition.y > 0 && !_despawning)
            {
                model.localPosition = new Vector3(model.localPosition.x, model.localPosition.y - 0.45f, model.localPosition.z);
                yield return new WaitForSeconds(0.05f);
            }
            GetComponent<Collider>().enabled = true;

            while (!_despawning)
            {
                switch (_currentState)
                {
                    //Practically idle, doesnt move but checks for player looking at it
                    case StalkerStates.Watching:
                        yield return new WaitForSeconds(0.1f);
                        if (PlayerMonsterManager.IsPlayerLookingAtObj(GetComponent<Collider>()))
                        {
                            _currentState = StalkerStates.Watched;
                            Attack();
                        }
                        else
                        {
                            _amountOfTimesNot++;
                            if(_amountOfTimesNot >= 100 + _moveRNG)
                            {
                                _currentState = StalkerStates.Moving;
                                _amountOfTimesNot = 0;
                            }
                        }
                        break;
                    //Prevent it from moving when the player is looking at it
                    case StalkerStates.Watched:
                        if (!PlayerMonsterManager.IsPlayerLookingAtObj(GetComponent<Collider>()))
                        {
                            _currentState = StalkerStates.Watching;
                            PlrRefs.inst.PlayerStatusEffects.ManageInsanityCauses("Stalker", true);
                        }
                        yield return new WaitForSeconds(0.1f);
                        break;
                    //Stalker actively teleporting to a different spot
                    case StalkerStates.Moving:
                        Soundsystem.PlaySound(_disappearSound, transform.position);

                        //Temporary teleport to -100
                        transform.position = new Vector3(-100,-100,-100);

                        yield return new WaitForSeconds(1f);

                        transform.position = GetRandomRoomPosition();
                        Soundsystem.PlaySound(_appearSound, transform.position);
                        _currentState = StalkerStates.Watching;
                        break;
                }
            }
        }

        public Vector3 GetRandomRoomPosition()
        {
            for (int i = 0; i < 15; i++)
            {
                if (i == 14)
                {
                    print("STALKER FAILED TO TP. THERE IS NO FALLBACK.");
                    return Vector3.zero;
                }

                float _ranX = Random.Range(_roomCorners[0].x, _roomCorners[1].x);
                float _ranY = Random.Range(_roomCorners[0].y + (_roomCorners[1].y/6), _roomCorners[1].y);
                float _ranZ = Random.Range(_roomCorners[0].z, _roomCorners[1].z);

                if (CheckIfAroundArea(_ranX, _player.position.x, _playerRadius) && CheckIfAroundArea(_ranY, _player.position.y, _playerRadius) && CheckIfAroundArea(_ranZ, _player.position.z, _playerRadius))
                {
                    print("Stalker tried to TP into the player");
                    continue;
                }

                if (_lastPosition != Vector3.zero)
                {
                    if (CheckIfAroundArea(_ranX, _lastPosition.x, _lastPosRadius) && CheckIfAroundArea(_ranY, _lastPosition.y, _lastPosRadius) && CheckIfAroundArea(_ranZ, _lastPosition.z, _lastPosRadius))
                    {
                        print("Stalker tried to TP into its last position!");
                        continue;
                    }
                }

                _lastPosition = new Vector3(_ranX, _ranY,_ranZ);
                i = 15;
            }

            return new Vector3(_lastPosition.x,_lastPosition.y, _lastPosition.z);
        }

        private bool CheckIfAroundArea(float value, float area, float range)
        {
            if (value < area + range && value > area - range) return true;
            return false;
        }

        private void Attack()
        {
            PlrRefs.inst.PlayerStatusEffects.ManageInsanityCauses("Stalker", false);
        }

        public override void DestroyMonster() 
        {
            gameObject.name = "Despawning";
            PlrRefs.inst.PlayerStatusEffects.ManageInsanityCauses("Stalker", true);
            _despawning = true;
            StartCoroutine(DespawnMonster());
        }

        public IEnumerator DespawnMonster()
        {
            //fly back up before destroying
            GetComponent<Collider>().enabled = false;
            Transform model = transform.GetChild(0);
            while (model.localPosition.y < 10)
            {
                model.localPosition = new Vector3(model.localPosition.x, model.localPosition.y + 0.3f, model.localPosition.z);
                yield return new WaitForSeconds(0.05f);
            }
            Destroy(gameObject);
        }
    }
}
