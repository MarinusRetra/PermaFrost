using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Stalker : Monster
    {
        private enum StalkerStates { Watching,Watched,Moving,Idle}
        private StalkerStates _currentState = StalkerStates.Watching;

        [SerializeField] private float _playerRadius;

        [SerializeField] private float _wallRadius;

        [SerializeField] private float _lastPosRadius;
        [SerializeField] private Vector3 _lastPosition;

        [SerializeField] private Vector3[] _roomCorners;

        private Transform _player;

        [SerializeField] private AudioClip _appearSound;
        [SerializeField] private AudioClip _disappearSound;

        private bool despawning = false;

        private int moveRNG = 0;

        void Start()
        {
            _player = PlayerMonsterManager.Instance.transform;
            //Get the rooms corners
            Vector3 _boundingCorner1 = CurrentRoom.transform.position - (CurrentRoom.transform.lossyScale / 2);
            Vector3 _boundingCorner2 = CurrentRoom.transform.position + (CurrentRoom.transform.lossyScale / 2);

            _roomCorners = new Vector3[] { new Vector3(_boundingCorner1.x + _wallRadius, _boundingCorner1.y + _wallRadius, _boundingCorner1.z + _wallRadius), new Vector3(_boundingCorner2.x - _wallRadius, _boundingCorner2.y + _wallRadius, _boundingCorner2.z - _wallRadius) };

            transform.position = GetRandomRoomPosition();
            Soundsystem.PlaySound(_appearSound, transform.position);
            _currentState = StalkerStates.Watching;
            moveRNG = Random.Range(-50, 100);
            StartCoroutine(HandleBehaviour());
        }

        int _amountOfTimesNot = 0;
        private IEnumerator HandleBehaviour()
        {
            Transform model = transform.GetChild(0);
            while (model.localPosition.y > 0 && !despawning)
            {
                model.localPosition = new Vector3(model.localPosition.x, model.localPosition.y - 0.3f, model.localPosition.z);
                yield return new WaitForSeconds(0.05f);
            }
            GetComponent<Collider>().enabled = true;
            while (!despawning)
            {
                switch (_currentState)
                {
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
                            if(_amountOfTimesNot >= 100 + moveRNG)
                            {
                                _currentState = StalkerStates.Moving;
                                _amountOfTimesNot = 0;
                            }
                        }
                            break;
                    case StalkerStates.Watched:
                        //prevent moving
                        if (!PlayerMonsterManager.IsPlayerLookingAtObj(GetComponent<Collider>()))
                        {
                            _currentState = StalkerStates.Watching;
                            PlayerStatusEffects.Instance.ManageInsanityCauses("Stalker", true);
                        }
                        yield return new WaitForSeconds(0.1f);
                        break;
                    case StalkerStates.Moving:
                        //begone
                        Soundsystem.PlaySound(_disappearSound, transform.position);
                        transform.position = new Vector3(-100,-100,-100);
                        yield return new WaitForSeconds(1f);
                        transform.position = GetRandomRoomPosition();
                        Soundsystem.PlaySound(_appearSound, transform.position);
                        _currentState = StalkerStates.Watching;
                        //beback
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
            PlayerStatusEffects.Instance.ManageInsanityCauses("Stalker", false);
        }

        public override void DestroyMonster() 
        {
            gameObject.name = "Despawning";
            PlayerStatusEffects.Instance.ManageInsanityCauses("Stalker", true);
            despawning = true;
            StartCoroutine(DespawnMonster());
        }

        public IEnumerator DespawnMonster()
        {
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
