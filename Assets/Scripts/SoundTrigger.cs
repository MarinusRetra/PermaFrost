using UnityEngine;

namespace Gameplay
{
    public class SoundTrigger : MonoBehaviour
    {
        [SerializeField] private bool _disabledAfterPlayed = false;
        private bool _hasPlayed = false;
        [SerializeField] private AudioClip _audioClip;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !_hasPlayed)
            {
                if (_disabledAfterPlayed) { _hasPlayed = true; }
                Soundsystem.PlaySound(_audioClip, transform.position);
            }
        }
    }
}
