using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Gameplay
{
    public class CutsceneManager : MonoBehaviour
    {
        public static CutsceneManager instance;
        [SerializeField] private GameObject _ui;
        [SerializeField] private Image _fadeImage;
        [SerializeField] private GameObject _startingCutscene;
        [SerializeField] private TimelineAsset _startingTimeline;
        [SerializeField] private TimelineAsset _endingTimeline;
        private GameObject _player;
        [SerializeField] private GameObject _backgroundSound;
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

            ForceStopStartingCutscene();
        }
        public void ForceStopStartingCutscene()
        {
            if (!_startingCutscene.activeSelf) return;
            //turn player back on
            _ui.SetActive(true);

            StartCoroutine(FadeScreen(0.3f, 1, () => {
                _startingCutscene.SetActive(false);
                _player.GetComponent<PlayerController>().enabled = true;

                _backgroundSound.SetActive(true);
                FindAnyObjectByType<Generation>().FastLoading = true;
            }));

        }

        public IEnumerator FadeScreen(float fadeTime, float darkTime, Action betweenSceneCode = null)
        {
            _fadeImage.gameObject.SetActive(true);

            // 1 = 0.1f
            // 2 = 0.05f
            for(int i = 0; i < fadeTime * 20;  i++)
            {
                float amountToGo = (0.05f / fadeTime) * (i + 1);

                _fadeImage.color = new Color(0, 0, 0, amountToGo);
                yield return new WaitForSeconds(0.05f);
            }
            betweenSceneCode?.Invoke();
            yield return new WaitForSeconds(darkTime);
            for (int i = 0; i < fadeTime * 20; i++)
            {
                float amountToGo = (0.05f / fadeTime) * (i + 1);

                _fadeImage.color = new Color(0, 0, 0, 1 - amountToGo);
                yield return new WaitForSeconds(0.05f);
            }
            _fadeImage.gameObject.SetActive(false);
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

            _backgroundSound.SetActive(false);
            _ui.SetActive(false);
            yield return new WaitForSeconds((float)_endingTimeline.duration);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(0);
        }
    }
}
