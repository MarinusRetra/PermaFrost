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
            _player = PlayerMonsterManager.Instance.transform;
        }
        public static void PlaySound(AudioClip _clip, Vector3 location)
        {
            _instance.PlaySoundInArea(_clip,location);
        }

        public static void PlaySound(AudioClip _clip)
        {
            _instance.PlaySoundOnPlayer(_clip);
        }

        public void PlaySoundInArea(AudioClip _clip, Vector3 location)
        {
            print("Tried to sound ");
            GameObject _currentSource = Instantiate(SoundSourcePrefab);
            AudioSource _source = _currentSource.GetComponent<AudioSource>();
            _source.clip = _clip;

            _currentSource.transform.position = location;
        }

        public void PlaySoundOnPlayer(AudioClip _clip)
        {
            GameObject _currentSource = Instantiate(SoundSourcePrefab);
            AudioSource _source = _currentSource.GetComponent<AudioSource>();
            _source.clip = _clip;
            _currentSource.transform.parent = _player;
        }
    }
}
