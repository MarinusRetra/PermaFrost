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
            _player = PlrRefs.inst.transform;
        }

        /// <summary>
        /// Play sound at a specific location
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="location"></param>
        /// <param name="_loop"></param>
        /// <returns></returns>
        public static GameObject PlaySound(AudioClip _clip, Vector3 location, bool _loop = false, bool _randomizePitch = false, float _randomValue = 0.1f)
        {
            return _instance.PlaySoundInArea(_clip,location,_loop,_randomizePitch,_randomValue);
        }

        /// <summary>
        /// Play sound on player
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_loop"></param>
        /// <returns></returns>
        public static GameObject PlaySound(AudioClip _clip, bool _loop = false, bool _randomizePitch = false, float _randomValue = 0.1f)
        {
            return _instance.PlaySoundOnPlayer(_clip,_loop, _randomizePitch, _randomValue);
        }

        public GameObject PlaySoundInArea(AudioClip _clip, Vector3 location, bool _loop = false, bool _randomizePitch = false, float _randomValue = 0.1f)
        {
            GameObject _currentSource = Instantiate(SoundSourcePrefab);
            AudioSource _source = _currentSource.GetComponent<AudioSource>();
            _source.clip = _clip;

            _currentSource.transform.position = location;
            if (_randomizePitch)
            {
                _source.pitch = 1 + Random.Range(-_randomValue, _randomValue);
            }

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

        public GameObject PlaySoundOnPlayer(AudioClip _clip, bool _loop = false, bool _randomizePitch = false, float _randomValue = 0.1f)
        {
            GameObject _currentSource = PlaySoundInArea(_clip, _player.position, _loop, _randomizePitch, _randomValue);
            _currentSource.transform.parent = _player;
            return _currentSource;
        }
    }
}
