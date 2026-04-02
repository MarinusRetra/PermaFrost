using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class MusicBox : MonoBehaviour
    {
        private MeshRenderer outlineVisual;
        [SerializeField] private Vector2 _boxRechargeTimes;
        private PlayerStatusEffects _playerEffects;

        [Header("Sound")]
        private GameObject _currentAudioSource;
        [SerializeField] private AudioClip _chargeClip;
        [SerializeField] private AudioClip _musicClip;
        void Start()
        {
            _playerEffects = PlrRefs.inst.PlayerStatusEffects;
            outlineVisual = GetComponent<MeshRenderer>();
            SilenceBox();
        }

        //Turn off the box, with all sound stopping
        public void SilenceBox(bool _startANew = true)
        {
            if (!outlineVisual.enabled) return;
            Destroy(_currentAudioSource);
            outlineVisual.enabled = false;

            if (_startANew)
            {
                StartCoroutine(ChargeBox());
            }
        }

        //Turn on box
        public void StartBox()
        {
            if (!outlineVisual.enabled) return;
            _playerEffects.ManageInsanityCauses("Music", false);
            _currentAudioSource = Soundsystem.PlaySound(_musicClip, transform.position,true);
        }

        private IEnumerator ChargeBox()
        {
            _playerEffects.ManageInsanityCauses("Music", true);
            yield return new WaitForSeconds(Random.Range(_boxRechargeTimes.x,_boxRechargeTimes.y));
            outlineVisual.enabled = true;
            _currentAudioSource = Soundsystem.PlaySound(_chargeClip, transform.position);
            //Charge sfx
            yield return new WaitForSeconds(4);
            Destroy(_currentAudioSource);
            //Actually play
            StartBox();
        }
    }
}
