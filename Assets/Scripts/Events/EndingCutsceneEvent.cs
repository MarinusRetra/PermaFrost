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
            PlrRefs.inst.Camera.transform.parent = cut.transform;
            PlrRefs.inst.gameObject.SetActive(false);
            CutsceneManager.instance.StartEndingCutscene();
            return true;
        }
        public override bool Exited(CarriageClass room) { return true; }
        public override bool Triggered(CarriageClass room) { return true; }

        public override bool Generated(CarriageClass room) { return true; }
        public override bool CallForDeletion(CarriageClass room) { return true; }
    }
}
