using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class MusicBox : MonoBehaviour
    {
        private MeshRenderer outlineVisual;
        [SerializeField] private Vector2 boxRechargeTimes;
        private PlayerStatusEffects _playerEffects;

        public GameObject _currentAudioSource;
        [SerializeField] private AudioClip _chargeClip;
        [SerializeField] private AudioClip _musicClip;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            outlineVisual = GetComponent<MeshRenderer>();
            SilenceBox();
            _playerEffects = PlayerStatusEffects.Instance;
        }
        private void Update()
        {
            if (_playerEffects.transform.position.z > transform.position.z + 5) { gameObject.SetActive(false); }
        }

        public void SilenceBox()
        {
            if (!outlineVisual.enabled) return;
            Destroy(_currentAudioSource);
            outlineVisual.enabled = false;
            StartCoroutine(ChargeBox());
        }

        public void StartBox()
        {
            if (!outlineVisual.enabled) return;
            _playerEffects.ManageInsanityCauses("Music", false);
            _currentAudioSource = Soundsystem.PlaySound(_musicClip, transform.position,true);
        }

        private IEnumerator ChargeBox()
        {
            //let player status effects load
            yield return new WaitForSeconds(0.1f);
            _playerEffects.ManageInsanityCauses("Music", true);
            yield return new WaitForSeconds(Random.Range(boxRechargeTimes.x,boxRechargeTimes.y));
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
