using UnityEngine;

namespace Gameplay
{
    public class Soundsystem : MonoBehaviour
    {
        private static Soundsystem _instance;
        public GameObject SoundSourcePrefab;
        private Transform _player;

        private void Start()
        {
            _instance = this;
            _player = PlayerStatusEffects.Instance.transform;
        }
        public static GameObject PlaySound(AudioClip _clip, Vector3 location, bool _loop = false)
        {
            return _instance.PlaySoundInArea(_clip,location,_loop);
        }

        public static GameObject PlaySound(AudioClip _clip, bool _loop = false)
        {
            return _instance.PlaySoundOnPlayer(_clip,_loop);
        }

        public GameObject PlaySoundInArea(AudioClip _clip, Vector3 location, bool _loop = false)
        {
            GameObject _currentSource = Instantiate(SoundSourcePrefab);
            AudioSource _source = _currentSource.GetComponent<AudioSource>();
            _source.clip = _clip;

            _currentSource.transform.position = location;

            if (_loop)
            {
                _source.loop = true;
            }
            else
            {
                Destroy(_currentSource, _source.clip.length);
            }

            _source.Play();

            return _currentSource;
        }

        public GameObject PlaySoundOnPlayer(AudioClip _clip, bool _loop = false)
        {
            GameObject _currentSource = Instantiate(SoundSourcePrefab);
            AudioSource _source = _currentSource.GetComponent<AudioSource>();
            _source.clip = _clip;
            _currentSource.transform.parent = _player;

            if (_loop)
            {
                _source.loop = true;
            }
            else
            {
                Destroy(_currentSource, _source.clip.length);
            }

            _source.Play();

            return _currentSource;
        }
    }
}
