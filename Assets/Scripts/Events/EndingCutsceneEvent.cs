using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/EndingEvent")]
    public class EndingCutsceneEvent : EventClass
    {
        [SerializeField]
        private GameObject _endingCutscenePrefab;
        [SerializeField] private TimelineAsset _endingTimeline;
        public override bool Entered(CarriageClass room)
        {
            //turn off the player and start the cutscene
            PlrRefs.inst.Camera.transform.parent = CutsceneManager.instance.transform;
            GameObject scene = null;
            CutsceneManager.instance.StartCutscene(_endingCutscenePrefab,_endingTimeline,() => { PlrRefs.inst.gameObject.SetActive(false); },null,() => { SceneManager.LoadScene(0); }, out scene, true,true,true, new Vector3(300, 300, 300));
            return true;
        }
        public override bool Exited(CarriageClass room) { return true; }
        public override bool Triggered(CarriageClass room) { return true; }

        public override bool Generated(CarriageClass room) { return true; }
        public override bool CallForDeletion(CarriageClass room) { return true; }
    }
}
