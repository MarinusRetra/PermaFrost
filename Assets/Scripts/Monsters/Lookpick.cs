using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Lookpick : Monster
    {
        private Collider _collider;
        private PlayerMonsterManager _pmm;
        private Animator _animator;
        [SerializeField] private Transform _transform;
        [SerializeField] private float _moveSpeed = 0.2f;
        [SerializeField] private float _returnSpeed = -0.6f;

        private enum lookStates { Moving, Idle }
        private lookStates _currentState = lookStates.Idle;
        private void Start()
        {
            _collider = GetComponent<Collider>();
            _animator = GetComponentInParent<Animator>();
            _pmm = PlayerMonsterManager.Instance;
            _animator.enabled = false;
            Aggro();
            _animator.enabled = true;
            StartCoroutine(HandleMovement());
        }

        private IEnumerator HandleMovement()
        {
            _animator.Play("Move");
            while (true)
            {
                switch (_currentState)
                {
                    case lookStates.Moving:
                        yield return new WaitForSeconds(0.05f);
                        if (PlayerMonsterManager.IsPlayerLookingAtObj(_collider))
                        {
                            _animator.SetFloat("direction", _returnSpeed);
                            print("BBBBBBBBBBBB");
                        }
                        else
                        {
                            _animator.SetFloat("direction", _moveSpeed);
                            print("AAAAAAAAAAAAAA");
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

            Vector3 _playerPos = _pmm.transform.position;
            _transform.position = new Vector3(_playerPos.x,_playerPos.y-1, _playerPos.z - 14);

            _currentState = lookStates.Moving;
        }

        public override void Deaggro()
        {
            _animator.Play("Move", -1, 0);
            print("SWAG");

            _currentState = lookStates.Moving;
        }
    }
}
