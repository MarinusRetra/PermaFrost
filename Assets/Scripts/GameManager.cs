using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        [SerializeField] private GameObject _pauseMenu;

        private void Start()
        {
            _input.PauseEvent += HandlePause;
            _input.ResumeEvent += HandleResume;
        }

        private void HandlePause()
        {
            _pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        private void HandleResume()
        {
            _pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
