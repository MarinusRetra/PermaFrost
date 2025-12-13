using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/EndingEvent")]
    public class EndingCutsceneEvent : EventClass
    {
        [SerializeField]
        private GameObject _endingCutscenePrefab;
        public override bool Entered(CarriageClass room)
        {
            //turn off the player and start the cutscene
            GameObject cut =  Instantiate(_endingCutscenePrefab);
            cut.transform.position = new Vector3(300, 300, 300);
            PlayerStatusEffects.Instance.transform.Find("CamBrain").parent = cut.transform;
            PlayerStatusEffects.Instance.gameObject.SetActive(false);
            CutsceneManager.instance.StartEndingCutscene();
            return true;
        }
        public override bool Exited(CarriageClass room) { return true; }
        public override bool Triggered(CarriageClass room) { return true; }

        public override bool Generated(CarriageClass room) { return true; }
        public override bool CallForDeletion(CarriageClass room) { return true; }
    }
}
