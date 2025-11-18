using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
        [SerializeField] VolumeProfile _postProcessing;
        private Vignette vignette;
        private enum LookStates { Moving, Idle }
        private LookStates _currentState = LookStates.Idle;
        private void Start()
        {
            _postProcessing.TryGet(out vignette);
            vignette.intensity.overrideState = true;
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
                    case LookStates.Moving:
                        yield return new WaitForSeconds(0.05f);
                        if (PlayerMonsterManager.IsPlayerLookingAtObj(_collider))
                        {
                            _animator.SetFloat("direction", _returnSpeed);
                        }
                        else if(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                        {
                            _animator.SetFloat("direction", _moveSpeed);
                        }
                        else
                        {
                            _animator.SetFloat("direction", 0);
                        }
                        vignette.intensity.value = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f - 0.2f;
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
            _transform.position = new Vector3(_playerPos.x,_playerPos.y-1, _playerPos.z - 10);

            _currentState = LookStates.Moving;
        }

        public override void Deaggro()
        {
            _animator.Play("Move", -1, 0);
            vignette.intensity.value = 0.2f;
        }

        void OnDestroy()
        {
            vignette.intensity.value = 0.2f;
            vignette.intensity.overrideState = false;
        }
    }
}
