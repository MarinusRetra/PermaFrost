using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Gameplay
{
    public class VolumeSettings : MonoBehaviour
    {
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer _audioMixer;

        [Header("Volume Sliders")]
        [SerializeField] private Slider _mainVolumeSlider;
        public void SetMainVolume()
        {
            float volume = _mainVolumeSlider.value;
            _audioMixer.SetFloat("Main Volume", Mathf.Log10(volume) * 20);
        }

    }
}
