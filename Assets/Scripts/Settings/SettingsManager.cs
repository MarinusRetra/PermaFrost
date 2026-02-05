using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Gameplay
{
    public class SettingsManager : MonoBehaviour
    {

        public static float cameraSensitivity;
        [SerializeField] private Slider camSensitivitySlider;
        public float defaultSensitivity = 0.4f;

        private void Start()
        {
            if (FindAnyObjectByType<PlayerController>())
            {
                FindAnyObjectByType<PlayerController>()._sensitivity = cameraSensitivity;
            }
            if (camSensitivitySlider)
            {
                camSensitivitySlider.value = cameraSensitivity;
            }
        }

        public void SetSensitivity(float number)
        {
            cameraSensitivity = number;
        }

        public void DefaultSensitivity()
        {
            cameraSensitivity = defaultSensitivity;
            camSensitivitySlider.value = defaultSensitivity;
        }

        public void SetToDefaults()
        {
            DefaultSensitivity();
            DefaultVolume();
        }

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer _audioMixer;

        [Header("Volume Sliders")]
        [SerializeField] private Slider _mainVolumeSlider;

        [SerializeField] private float defaultVolume = 0.5f;
        public void SetMainVolume()
        {
            float volume = _mainVolumeSlider.value;
            _audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        }

        public void DefaultVolume()
        {
            _mainVolumeSlider.value = defaultVolume;
            _audioMixer.SetFloat("Master", Mathf.Log10(defaultVolume) * 20);
        }
    }
}
