using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        private GameObject _player;
        [SerializeField] private GameObject _backgroundSound;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        private void OnEnable()
        {
            _input.SkipEvent += SkipCutscene;
        }

        void Start()
        {
            instance = this;
            _player = PlrRefs.inst.gameObject;
            StartCutscene(_startingCutscene, _startingTimeline, null, () => { Generation.mainInstance.FastLoading = true; }, () => { _backgroundSound.SetActive(true); },out GameObject scene);
        }

        private GameObject currentAnimPlaying = null;
        private Action onAnimSkip;
        private Action onAnimEnd;
        private bool animEndFreeMouse = true;
        private int cutsceneID = 0;
        private bool inCutscene = false;
        public void StartCutscene(GameObject pref, TimelineAsset timeL, Action onStart, Action onSkip, Action onEnd, out GameObject spawnedScene, bool fadeAtStart = false, bool fadeAtEnd = false, bool freeMouseOnEnd = false, Vector3 animLocation = new(), Quaternion animRotation = new())
        {
            //needs to be anything before a return
            spawnedScene = null;

            //if there already is an animation or if there is no object
            if (currentAnimPlaying != null || pref == null || inCutscene) { return; }

            inCutscene = true;
            //if another cutscene gets started during the waiting time of another, it will know.
            cutsceneID += 1;

            //Fade based on FadeAtStart
            StartCoroutine(FadeScreen((fadeAtStart ? 0.3f : 0), (fadeAtStart ? 1 : 0), () =>
            {
                //Spawn animation
                if (pref.scene.name == null)
                {
                    currentAnimPlaying = Instantiate(pref,animLocation,animRotation);
                }
                else
                {
                    currentAnimPlaying = pref;
                }
                _cutsceneUi.SetActive(true);

                //turn off player
                _input.SetCutsceneActions();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _ui.SetActive(false);
                PlrRefs.inst.PlayerController.enabled = false;

                //These values are set for skipping
                onAnimSkip = onSkip;
                onAnimEnd = onEnd;
                animEndFreeMouse = freeMouseOnEnd;

                if (onStart != null) { onStart(); }
                StartCoroutine(WaitCutsceneTime(timeL, onEnd, fadeAtEnd, freeMouseOnEnd));
            }));
        }

        private IEnumerator WaitCutsceneTime(TimelineAsset timeL, Action onEnd, bool fadeAtEnd, bool freeMouseOnEnd)
        {
            //if another cutscene gets started during the waiting time of another, it will know.
            int thisCutsceneID = cutsceneID;

            yield return new WaitForSeconds((float)timeL.duration - (fadeAtEnd ? 0.1f:0.4f));

            if(cutsceneID != thisCutsceneID) { yield break; }
            EndCutscene(onEnd, fadeAtEnd, freeMouseOnEnd);
        }

        private void EndCutscene(Action onEnd, bool fadeAtEnd, bool freeMouseOnEnd)
        {
            if(currentAnimPlaying == null) { return; }

            //Fade based on fadeAtEnd
            StartCoroutine(FadeScreen((fadeAtEnd ? 0.3f : 0), (fadeAtEnd ? 1 : 0), () =>
            {
                _cutsceneUi.SetActive(false);

                //turn on player
                _input.SetGameplayActions();
                if (!freeMouseOnEnd)
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

                //reset all cutscene stuff
                Destroy(currentAnimPlaying);
                currentAnimPlaying = null;
                onAnimEnd = null;
                animEndFreeMouse = true;

                if (onEnd != null) { onEnd(); }
                inCutscene = false;
            }));
        }

        private void SkipCutscene()
        {
            if(onAnimSkip != null) { onAnimSkip(); }
            onAnimSkip = null;
            EndCutscene(onAnimEnd,true,animEndFreeMouse);
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

        public static void StartCutsceneStatic(string name)
        {
            instance.StartCutsceneByName(name);
        }

        /// <summary>
        /// Used for cutscenes that cannot be hosted elsewhere, aka homeless cutscenes
        /// </summary>
        /// <param name="name"></param>
        public void StartCutsceneByName(string name)
        {
            switch (name)
            {
                case "PaintingSecret":
                    StartCutscene(_paintingCutscenePrefab, _paintingTimeline, () =>
                    {
                        GameObject fakeRoom = PlrRefs.inst.PlayerController.CurrentRoom;
                        CarriageClass fakeCarriage = fakeRoom.GetComponent<CarriageClass>();

                        //delete all current events
                        List<EventClass> even = fakeCarriage._selectedEventClasses;
                        for (int i = 0; i < even.Count; i++)
                        {
                            even[i].Exited(fakeCarriage);
                            even[i].CallForDeletion(fakeCarriage);
                        }
                        _player.GetComponent<Rigidbody>().isKinematic = true;

                        originalRoomLocal = fakeRoom.transform.position;
                        fakeRoom.transform.position = new Vector3(100, 100, 100);
                    }, null, () =>
                    {
                        PlrRefs.inst.PlayerController.CurrentRoom.transform.position = originalRoomLocal;
                        _player.GetComponent<Rigidbody>().isKinematic = false;

                        GameObject newGen = Instantiate(_baseGen, new Vector3(200, 0, 200), Quaternion.identity);

                        Generation paintingRoomsGenerator = newGen.GetComponent<Generation>();
                        paintingRoomsGenerator.player = _player;
                        paintingRoomsGenerator.AmountOfRooms = 5;
                        paintingRoomsGenerator.Rooms = _paintingRooms;
                        paintingRoomsGenerator.FastLoading = true;
                    }, out GameObject scene, true, true, false, new Vector3(100, 100, 100), Quaternion.identity);
                    break;
            }
        }

        //Values for homeless cutscenes
        [Header("Painting Cutscene")]
        [SerializeField] private GameObject _paintingCutscenePrefab;
        [SerializeField] private TimelineAsset _paintingTimeline;
        [SerializeField] private RoomTypeScriptable _paintingRooms;
        [SerializeField] private GameObject _baseGen;
        private Vector3 originalRoomLocal;
        private void OnDisable()
        {
            _input.SkipEvent -= SkipCutscene;
        }
    }
}
