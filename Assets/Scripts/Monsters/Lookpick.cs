using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Lookpick : Monster
    {
        private Collider _collider;
        private MeshRenderer _renderer;
        private PlayerMonsterManager _pmm;

        [SerializeField] private float _moveSpeed = 2.5f;
        [SerializeField] private float _returnSpeed = 5f;

        private enum lookStates { Moving, Idle }
        private lookStates _currentState = lookStates.Idle;
        private void Start()
        {
            _collider = GetComponent<Collider>();
            _renderer = GetComponent<MeshRenderer>();
            _pmm = PlayerMonsterManager.Instance;
            StartCoroutine(HandleMovement());
        }

        private float tooFar;
        private IEnumerator HandleMovement()
        {
            while (true)
            {
                switch (_currentState)
                {
                    case lookStates.Moving:
                        //td: comments, clean, ect.
                        yield return new WaitForSeconds(0.05f);
                        if (PlayerMonsterManager.IsPlayerLookingAtObj(GetComponent<Collider>()))
                        {
                            if(transform.position.z !>= tooFar)
                            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (_returnSpeed / 50));
                        }
                        else
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (_moveSpeed / 50));
                        }
                        continue;
                    default:
                        yield return new WaitForSeconds(0.1f);
                        break;
                }
            }
        }
        public override void Aggro()
        {
            _collider.enabled = true;
            _renderer.enabled = true;

            Vector3 _playerPos = _pmm.transform.position;
            transform.position = new Vector3(_playerPos.x,_playerPos.y, _playerPos.z - 5);
            tooFar = transform.position.z - 2;

            _currentState = lookStates.Moving;
        }

        public override void Deaggro()
        {
            //this practically turns lookpick off
            _collider.enabled = false;
            _renderer.enabled = false;

            _currentState = lookStates.Idle;
        }
    }
}
