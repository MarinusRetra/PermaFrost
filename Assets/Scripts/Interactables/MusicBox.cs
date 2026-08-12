using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class MusicBox : MonoBehaviour
    {
        private MeshRenderer outlineVisual;
        [SerializeField] private Vector2 _firstRechargeTimes;
        [SerializeField] private Vector2 _boxRechargeTimes;
        private PlayerStatusEffects _playerEffects;

        [Header("Sound")]
        private GameObject _currentAudioSource;
        [SerializeField] private AudioClip _chargeClip;
        [SerializeField] private AudioClip _musicClip;

        [Header("Animation")]
        [SerializeField] private Animator animator;

        public bool CanPlay = false;
        public bool isCharging = false;

        private bool firstTime = true;
        void Start()
        {
            firstTime = true;
            _playerEffects = PlrRefs.inst.PlayerStatusEffects;
            outlineVisual = GetComponent<MeshRenderer>();
        }

        //Turn off the box, with all sound stopping
        public void SilenceBox(bool _startANew = true)
        {
            if (!outlineVisual.enabled) return;
            Destroy(_currentAudioSource);
            outlineVisual.enabled = false;
            animator.SetBool("StartBox", false);
            isCharging = false;
            if (_startANew)
            {
                StartCoroutine(ChargeBox());
            }
        }

        public void SetBoxIdleState(bool idle)
        {
            if (idle)
            {
                Destroy(_currentAudioSource);
                SilenceBox(false);
                CanPlay = false;
                _playerEffects.ManageInsanityCauses("Music", true);
            }
            else
            {
                outlineVisual.enabled = false;
                StartCoroutine(ChargeBox());
                CanPlay = true;
            }
        }

        //Turn on box
        public void StartBox()
        {
            if (!outlineVisual.enabled || !CanPlay) return;
            _playerEffects.ManageInsanityCauses("Music", false);
            _currentAudioSource = Soundsystem.PlaySound(_musicClip, transform.position,true);
        }

        private IEnumerator ChargeBox()
        {
            if (isCharging) { yield break; }
            isCharging = true;
            _playerEffects.ManageInsanityCauses("Music", true);

            if (firstTime)
            {
                yield return new WaitForSeconds(Random.Range(_firstRechargeTimes.x, _firstRechargeTimes.y));
                firstTime = false;
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(_boxRechargeTimes.x, _boxRechargeTimes.y));
            }

            if (!CanPlay) { isCharging = false; yield break;}
            animator.SetBool("StartBox", true);
            outlineVisual.enabled = true;
            //Charge sfx
            _currentAudioSource = Soundsystem.PlaySound(_chargeClip, transform.position);
            yield return new WaitForSeconds(4);
            Destroy(_currentAudioSource);
            //Start draining sanity
            StartBox();
            isCharging = false;
        }
    }
}
