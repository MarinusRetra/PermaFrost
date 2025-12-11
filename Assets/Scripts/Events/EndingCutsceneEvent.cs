using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/EndingEvent")]
    public class EndingCutsceneEvent : EventClass
    {
        [SerializeField]
        private GameObject _endingCutscenePrefab;
        public override bool Entered(GameObject room)
        {
            //turn off the player and start the cutscene
            GameObject cut =  Instantiate(_endingCutscenePrefab);
            cut.transform.position = new Vector3(300, 300, 300);
            PlayerStatusEffects.Instance.transform.Find("CamBrain").parent = cut.transform;
            PlayerStatusEffects.Instance.gameObject.SetActive(false);
            CutsceneManager.instance.StartEndingCutscene();
            return true;
        }
        public override bool Exited(GameObject room) { return true; }
        public override bool Triggered(GameObject room) { return true; }

        public override bool Generated(GameObject room) { return true; }
        public override bool CallForDeletion(GameObject room) { return true; }
    }
}
