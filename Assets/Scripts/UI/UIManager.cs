using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

namespace Gameplay
{
    public class UIManager : MonoBehaviour
    {
		[SerializeField] private InputReader _input;

		[SerializeField] private GameObject _settingsPanel;
		bool _settingsToggle = false;

		private void Start()
		{
			if(_input) _input.ResumeEvent += OnResume;
		}

		public void OnResume()
        {
			gameObject.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			_input.SetGameplayActions();
		}

		public void SettingsToggle()
		{
			_settingsToggle = !_settingsToggle;
			_settingsPanel.SetActive(_settingsToggle);
		}

		public void LoadScene(int id) => SceneManager.LoadScene(id);
		public void QuitGame() => Application.Quit();

        private void OnDisable()
        {
            if (_input) _input.ResumeEvent -= OnResume;
        }
    }
}
