using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class NoiseMonkeyObject : MonoBehaviour
    {
        [SerializeField] private float _duration;
        private bool _isPlaying;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(AutoPlayTime());
        }

        private IEnumerator AutoPlayTime()
        {
            yield return new WaitForSeconds(_duration);
            if (!_isPlaying)
            {
                StartCoroutine(PlaySound());
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "Floor")
            {
                if (!_isPlaying)
                {
                    StartCoroutine(PlaySound());
                }
            }
        }

        int _totalTimes = 0;
        private IEnumerator PlaySound()
        {
            _isPlaying = true;
            PlayerMonsterManager.Instance.OverridePlayerSounds = true;
            GetComponent<Rigidbody>().isKinematic = true;
            while(_totalTimes < _duration * 100)
            {
                yield return new WaitForSeconds(0.01f);
                PlayerMonsterManager.MakeNoise(transform.position);
                _totalTimes++;
            }
            PlayerMonsterManager.Instance.OverridePlayerSounds = false;
            Destroy(gameObject);
        }
    }
}
