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
           GameObject cut =  Instantiate(_endingCutscenePrefab);
            cut.transform.position = new Vector3(300, 300, 300);
            return true;
        }
        public override bool Exited(GameObject room)
        {
            return true;
        }
        public override bool Triggered(GameObject room)
        {
            return true;
        }
    }
}
