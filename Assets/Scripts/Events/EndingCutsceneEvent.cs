using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

namespace Gameplay
{
    public class EndingCutsceneEvent : EventClass
    {
        //When room spawns in
        public override bool Generate(CarriageClass room) { return true; }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room) { return true; }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room) { return true; }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
        {
            return RepeatEnter(room);
        }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room)
        {
            //turn off the player and start the cutscene
            PlrRefs.inst.Camera.transform.parent = CutsceneManager.instance.transform;
            GameObject scene = null;
            EventAnimScriptable animEvent = scriptable as EventAnimScriptable;
            CutsceneManager.instance.StartCutscene(scriptable.SpawnablePrefab, animEvent.animTimeline, () => { PlrRefs.inst.gameObject.SetActive(false); }, null, () => { SceneManager.LoadScene(0); }, out scene, true, true, true, new Vector3(300, 300, 300));
            return true;
        }
        //First time completing room
        public override bool FirstExit(CarriageClass room) { return true; }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room) { return true; }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room) { return true; }
        //Getting far away from the room
        public override bool Recede(CarriageClass room) { return true; }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room) 
        {
            Debug.LogWarning("You just Call For Deletion'd the ending cutscene");
            Destroy(this);
            return true; 
        }
    }
}
