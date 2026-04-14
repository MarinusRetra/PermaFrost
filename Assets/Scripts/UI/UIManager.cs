using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

namespace Gameplay
{
    public class UIManager : MonoBehaviour
    {
		[SerializeField] private InputReader _input;
		[SerializeField] private StatTracker _statTrack;
		[SerializeField] private GameObject _settingsPanel;
		[SerializeField] private GameObject[] _StatNamesUI;
		[SerializeField] private GameObject[] _StatValuesUI;
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
		public void QuitGame()
		{
			_statTrack.SaveStats();
			Application.Quit();
		} 

        private void OnDisable()
        {
            if (_input) _input.ResumeEvent -= OnResume;
        }
    }
}
