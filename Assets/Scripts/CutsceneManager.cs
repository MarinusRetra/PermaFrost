using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

namespace Gameplay
{
    public class CutsceneManager : MonoBehaviour
    {
        public static CutsceneManager instance;
        [SerializeField] private GameObject _ui;
        [SerializeField] private TimelineAsset _startingTimeline;
        [SerializeField] private TimelineAsset _endingTimeline;
        private GameObject _player;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            instance = this;
            _player = PlayerStatusEffects.Instance.gameObject;
            if (FindAnyObjectByType<PlayableDirector>())
            {
                StartCoroutine(StartStartingCutscene());
            }
        }
        private IEnumerator StartStartingCutscene()
        {
            //turn off player
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _ui.SetActive(false);
            _player.GetComponent<PlayerController>().enabled = false;

            yield return new WaitForSeconds((float)_startingTimeline.duration);

            //turn player back on
            _ui.SetActive(true);
            _player.GetComponent<PlayerController>().enabled = true;
        }

        /// <summary>
        /// This function exists to call the function in places you cant start coroutines
        /// </summary>
        public void StartEndingCutscene()
        {
            StartCoroutine(StartEndingCutsceneCoroutine());
        }

        public IEnumerator StartEndingCutsceneCoroutine()
        {
            _ui.SetActive(false);
            yield return new WaitForSeconds((float)_endingTimeline.duration);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(0);
        }
    }
}
