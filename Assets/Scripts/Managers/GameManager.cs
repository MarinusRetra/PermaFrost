using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private StatTracker _statTrack;
        [SerializeField] private GameObject _resumeButton;
        [SerializeField] private EventSystem _eventSystem;

        private void Start()
        {
            _input.PauseEvent += HandlePause;
            _input.ResumeEvent += HandleResume;
        }

        private void HandlePause()
        {
            _pauseMenu.SetActive(true);
            _statTrack.LoadStats();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            if( _eventSystem != null && _resumeButton != null)
            {
                _eventSystem.SetSelectedGameObject(_resumeButton);
            }
        }
        private void HandleResume()
        {
            _pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            //print(_eventSystem.currentSelectedGameObject.name);
        }

        private void OnDisable()
        {
            _input.PauseEvent -= HandlePause;
            _input.ResumeEvent -= HandleResume;
        }
        private void OnDestroy()
        {
            _input.PauseEvent -= HandlePause;
            _input.ResumeEvent -= HandleResume;
        }
    }
}
