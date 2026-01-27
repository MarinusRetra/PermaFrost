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

            yield return new WaitForSeconds((float)_startingTimeline.duration - 0.1f);

            ForceStopStartingCutscene(false);
        }
        public void ForceStopStartingCutscene(bool fade = true)
        {
            if (!_startingCutscene.activeSelf) return;
            _ui.SetActive(true);

            StartCoroutine(FadeScreen(0.3f * (fade ? 1 : 0), 1 * (fade ? 1 : 0), () => {
                //turn player back on
                _startingCutscene.SetActive(false);
                _player.GetComponent<PlayerController>().enabled = true;

                _backgroundSound.SetActive(true);
                FindAnyObjectByType<Generation>().FastLoading = true;
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

            //do code that was given
            betweenSceneCode?.Invoke();
            yield return new WaitForSeconds(darkTime);

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

            _backgroundSound.SetActive(false);
            _ui.SetActive(false);
            yield return new WaitForSeconds((float)_endingTimeline.duration);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
            GameObject fakeRoom = _player.GetComponent<PlayerController>().CurrentRoom;
            Vector3 originalRoomPos = fakeRoom.transform.position;
            StartCoroutine(FadeScreen(0.3f, 1, () =>
            {
                spawnedScene = Instantiate(_paintingCutscenePrefab, new Vector3(100, 100, 100), new Quaternion(0, 0, 0, 0));
                _player.GetComponent<PlayerController>().enabled = false;
                _player.GetComponent<Rigidbody>().isKinematic = true;
                spawnedScene.transform.Find("RoomTemplate").gameObject.SetActive(false);
                fakeRoom.transform.position = spawnedScene.transform.position;
            }));

            yield return new WaitForSeconds((float)(_paintingTimeline.duration) - 0.3f);

            StartCoroutine(FadeScreen(0.3f, 1, () =>
            {
                fakeRoom.transform.position = originalRoomPos;
                _player.GetComponent<PlayerController>().enabled = true;
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
    }
}
