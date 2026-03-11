using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class NoiseMonkeyObject : MonoBehaviour
    {
        [SerializeField] private float _duration;
        private bool _isPlaying;
        private Rigidbody _body;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _body = GetComponent<Rigidbody>();
            StartCoroutine(AutoPlayTime());
        }

        private IEnumerator AutoPlayTime()
        {
            //play noise after a bit, so if it gets stuck it still works
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
                StartCoroutine(PlaySound());
            }
        }

        int _totalTimes = 0;
        private IEnumerator PlaySound()
        {
            if (_isPlaying) { yield return 0; }
            _isPlaying = true;
            PlayerMonsterManager.Instance.OverridePlayerSounds = true;
            _body.isKinematic = true;

            //this gets called so much to prevent allears from moving away to other noises
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
