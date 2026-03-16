using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Gameplay
{
    public class SettingsManager : MonoBehaviour
    {

        public static float cameraSensitivity = 1;
        [SerializeField] private Slider camSensitivitySlider;
        public float defaultSensitivity = 1;

        private void Start()
        {
            if (PlrRefs.inst.PlayerController)
            {
                PlrRefs.inst.PlayerController._sensitivity = cameraSensitivity * 0.4f;
            }
            if (camSensitivitySlider)
            {
                camSensitivitySlider.value = cameraSensitivity;
            }
        }

        public void SetSensitivity(float number)
        {
            cameraSensitivity = number;
            if (PlrRefs.inst.PlayerController)
            {
                PlrRefs.inst.PlayerController._sensitivity = cameraSensitivity * 0.4f;
            }
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

        public void SwitchWindowMode(int windowType)
        {
            //Self explanatory
            switch (windowType)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
        }
    }
}
