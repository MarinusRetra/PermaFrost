using System.IO;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Gameplay
{
    public class SettingsManager : MonoBehaviour
    {
        public static Settings currentSettings;

        [Header("General")]
        [SerializeField] private Slider camSensitivitySlider;
        [SerializeField] private Slider camFovSlider;
        [SerializeField] private Toggle camBobToggle;

        [Header("Graphics")]
        [SerializeField] private TMP_Dropdown windowModeDropdown;

        [Header("Audio")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Slider mainVolumeSlider;
        [SerializeField] private Slider backgroundVolumeSlider;

        private void Start()
        {
            CheckAndGetSettings();
            ApplySettings();
        }

        public void OpenSettings()
        {
            ApplySettingVisuals();
        }
        public void ApplySettings()
        {
            if(currentSettings == null) { return; }
            if (PlrRefs.inst && PlrRefs.inst.PlayerController)
            {
                PlrRefs.inst.PlayerController._sensitivity = currentSettings.camSensitivity * 0.4f;
                PlrRefs.inst.PlayerCamera.GetComponent<CinemachineCamera>().Lens.FieldOfView = currentSettings.camFOV;
                PlrRefs.inst.PlayerController.DoHeadBob = currentSettings.doCamHeadBob;
            }
            _audioMixer.SetFloat("Master", Mathf.Log10(currentSettings.masterVolume) * 20);
            _audioMixer.SetFloat("Background", Mathf.Log10(currentSettings.backgroundVolume) * 20);
            switch (currentSettings.windowMode)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
        }

        public void ApplySettingVisuals()
        {
            camSensitivitySlider.value = currentSettings.camSensitivity;
            camFovSlider.value = currentSettings.camFOV;
            camBobToggle.isOn = currentSettings.doCamHeadBob;

            mainVolumeSlider.value = currentSettings.masterVolume;
            backgroundVolumeSlider.value = currentSettings.backgroundVolume;

            windowModeDropdown.value = currentSettings.windowMode;
        }

        public void SaveSettings()
        {
            //Get settings
            Settings newlySettings = new Settings();
            newlySettings.camSensitivity = camSensitivitySlider.value;
            newlySettings.doCamHeadBob = camBobToggle.isOn;
            newlySettings.camFOV = camFovSlider.value;

            newlySettings.masterVolume = mainVolumeSlider.value;
            newlySettings.backgroundVolume = backgroundVolumeSlider.value;

            newlySettings.windowMode = windowModeDropdown.value;

            //Save em

            string jsonString = JsonUtility.ToJson(newlySettings);
            print(jsonString);

            using (StreamWriter writer = new StreamWriter(Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Settings.json"))
            {
                writer.Write(jsonString);
            }

            currentSettings = newlySettings;
            ApplySettings();
        }

        public Settings GetSettings()
        {
            string oldSettings = string.Empty;

            using (StreamReader reader = new StreamReader(Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Settings.json"))
            {
                oldSettings = reader.ReadToEnd();
            }
            return JsonUtility.FromJson<Settings>(oldSettings);
        }

        public void SetSettingsToDefault()
        {
            TextAsset json = Resources.Load<TextAsset>("DefaultSettings");
            Settings defSettings = JsonUtility.FromJson<Settings>(json.text);

            camSensitivitySlider.value = defSettings.camSensitivity;
            camFovSlider.value = defSettings.camFOV;
            camBobToggle.isOn = defSettings.doCamHeadBob;

            mainVolumeSlider.value = defSettings.masterVolume;
            backgroundVolumeSlider.value = defSettings.backgroundVolume;

            windowModeDropdown.value = defSettings.windowMode;
        }

        public Settings CheckAndGetSettings()
        {
            if (currentSettings == null) { currentSettings = GetSettings(); }
            return currentSettings;
        }
    }
    public class Settings
    {
        public float camSensitivity;
        public bool doCamHeadBob;
        public float camFOV;

        public float masterVolume;
        public float backgroundVolume;

        public int windowMode;
    }
}
