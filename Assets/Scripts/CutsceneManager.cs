using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private InputReader _input;
        [SerializeField] private GameObject _ui;
        [SerializeField] private GameObject _cutsceneUi;
        [SerializeField] private Image _fadeImage;
        [SerializeField] private GameObject _startingCutscene;
        [SerializeField] private TimelineAsset _startingTimeline;
        [SerializeField] private TimelineAsset _endingTimeline;
        private GameObject _player;
        [SerializeField] private GameObject _backgroundSound;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        private void OnEnable()
        {
            _input.SkipEvent += HandleSkip;
        }

        void Start()
        {
            instance = this;
            _player = PlrRefs.inst.gameObject;
            if (FindAnyObjectByType<PlayableDirector>())
            {
                StartCoroutine(StartStartingCutscene());
            }
        }

        private void StartCutscene()
        {
            _cutsceneUi.SetActive(true);
            _input.SetCutsceneActions();
            //turn off player
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _ui.SetActive(false);
            PlrRefs.inst.PlayerController.enabled = false;
        }

        private void EndCutscene(bool freeMouse)
        {
            _cutsceneUi.SetActive(false);
            _input.SetGameplayActions();
            //turn off player
            if (!freeMouse)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            _ui.SetActive(true);
            PlrRefs.inst.PlayerController.enabled = true;
        }

        private void SkipCutscene()
        {
            //implement later
        }
        private IEnumerator StartStartingCutscene()
        {
            StartCutscene();

            yield return new WaitForSeconds((float)_startingTimeline.duration - 0.1f);

            StopStartingCutscene(false);
        }

        public void HandleSkip()
        {
            StopStartingCutscene();
        }
        public void StopStartingCutscene(bool fade = true)
        {
            if (!_startingCutscene.activeSelf) return;

            StartCoroutine(FadeScreen(0.3f * (fade ? 1 : 0), 1 * (fade ? 1 : 0), () => {
                EndCutscene(false);
                _backgroundSound.SetActive(true);
                Generation.mainInstance.FastLoading = true;
            }));
        }

        public IEnumerator FadeScreen(float fadeTime, float darkTime, Action betweenSceneCode = null)
        {
            //if fade is set to 0 then just do the action
            if(fadeTime == 0 && darkTime == 0)
            {
                betweenSceneCode?.Invoke();
                yield break;
            }
            _fadeImage.gameObject.SetActive(true);

            //fade to black
            for(int i = 0; i < fadeTime * 20;  i++)
            {
                float amountToGo = (0.05f / fadeTime) * (i + 1);

                _fadeImage.color = new Color(0, 0, 0, amountToGo);
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(darkTime/2);
            //do code that was given
            betweenSceneCode?.Invoke();
            yield return new WaitForSeconds(darkTime/2);

            //fade back
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

            StartCutscene();
            yield return new WaitForSeconds((float)_endingTimeline.duration);
            EndCutscene(true);
            SceneManager.LoadScene(0);
        }

        public static void StartCutsceneStatic(string name)
        {
            instance.StartCutscene(name);
        }

        public void StartCutscene(string name)
        {
            switch (name)
            {
                case "PaintingSecret":
                    StartCoroutine(StartSecretCutscene());
                    break;
            }
        }


        [SerializeField] private GameObject _paintingCutscenePrefab;
        [SerializeField] private TimelineAsset _paintingTimeline;
        [SerializeField] private RoomTypeScriptable _paintingRooms;
        [SerializeField] private GameObject _baseGen;
        public IEnumerator StartSecretCutscene()
        {
            GameObject spawnedScene = null;
            GameObject fakeRoom = PlrRefs.inst.PlayerController.CurrentRoom;

            //delete all current events
            List<EventClass> even = fakeRoom.GetComponent<CarriageClass>()._selectedEventClasses;
            for (int i = 0; i < even.Count; i++)
            {
                even[i].Exited(fakeRoom.GetComponent<CarriageClass>());
                even[i].CallForDeletion(fakeRoom.GetComponent<CarriageClass>());
            }

            Vector3 originalRoomPos = fakeRoom.transform.position;
            StartCoroutine(FadeScreen(0.3f, 1, () =>
            {
                spawnedScene = Instantiate(_paintingCutscenePrefab, new Vector3(100, 100, 100), new Quaternion(0, 0, 0, 0));
                PlrRefs.inst.PlayerController.enabled = false;
                _player.GetComponent<Rigidbody>().isKinematic = true;
                fakeRoom.transform.position = spawnedScene.transform.position;
            }));

            yield return new WaitForSeconds((float)(_paintingTimeline.duration) - 0.3f);

            StartCoroutine(FadeScreen(0.3f, 1, () =>
            {
                fakeRoom.transform.position = originalRoomPos;
                PlrRefs.inst.PlayerController.enabled = true;
                _player.GetComponent<Rigidbody>().isKinematic = false;
                Destroy(spawnedScene);

                GameObject newGen = Instantiate(_baseGen, new Vector3(200, 0, 200), new Quaternion(0, 0, 0, 0));

                Generation paintingRoomsGenerator = newGen.GetComponent<Generation>();
                paintingRoomsGenerator.player = _player;
                paintingRoomsGenerator.AmountOfRooms = 5;
                paintingRoomsGenerator.Rooms = _paintingRooms;
                paintingRoomsGenerator.FastLoading = true;
            }));
        }
        private void OnDisable()
        {
            _input.SkipEvent -= HandleSkip;
        }
    }
}
