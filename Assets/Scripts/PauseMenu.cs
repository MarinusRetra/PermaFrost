using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class PauseMenu : MonoBehaviour
    {
		[SerializeField] private InputReader _input;

		[SerializeField] private Button _resumeButton;
		[SerializeField] private Button _settingsButton;
		[SerializeField] private Button _quitSettingsButton;
		[SerializeField] private Button _quitButton;

		[SerializeField] private GameObject _settingsPanel;

		bool _settingsToggle = false;

		private void Start()
		{
			_input.ResumeEvent += OnResume;
		}

		// Update is called once per frame
		void Update()
        {
        
        }

		private void OnEnable()
		{
			_input.ResumeEvent += OnResume;
			_resumeButton.onClick.AddListener(OnResume);
			_settingsButton.onClick.AddListener(SettingsToggle);
			_quitSettingsButton.onClick.AddListener(SettingsToggle);
			_quitButton.onClick.AddListener(OnQuitToMenu);
		}

		private void OnDisable()
		{
			_input.ResumeEvent -= OnResume;
			_resumeButton.onClick.RemoveAllListeners();
			_settingsButton.onClick.RemoveAllListeners();
			_quitSettingsButton.onClick.RemoveAllListeners();
			_quitButton.onClick.RemoveAllListeners();
		}

		private void OnResume()
        {
			gameObject.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			_input.SetGameplayActions();
		}

		private void SettingsToggle()
		{
			_settingsToggle = !_settingsToggle;
			_settingsPanel.SetActive(_settingsToggle);
		}

		private void OnQuitToMenu() => SceneManager.LoadScene(0);
    }
}
