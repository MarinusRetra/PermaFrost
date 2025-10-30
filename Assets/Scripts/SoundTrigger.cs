using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class SoundTrigger : MonoBehaviour
    {
        [SerializeField] private bool _disabledAfterPlayed = false;
        [SerializeField] private bool _loop = false;
        [SerializeField] private float _cooldown = 1f;
        private bool _hasPlayed = false;
        private bool _onCooldown;
        [SerializeField] private AudioClip _audioClip;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !_hasPlayed && !_onCooldown)
            {
                StartCoroutine(HandleCooldown());
                if (_disabledAfterPlayed) { _hasPlayed = true; }
                Soundsystem.PlaySound(_audioClip, transform.position,_loop);
            }
        }

        private IEnumerator HandleCooldown()
        {
            _onCooldown = true;
            yield return new WaitForSeconds(_cooldown);
            _onCooldown = false;
        }
    }
}
