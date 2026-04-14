using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class WindowEvent : EventClass
    {
        //Variables
        private GameObject spawnedFreezingArea;
        private EventWindowScriptable windEvent;

        //When room spawns in
        public override bool Generate(CarriageClass room)
        {
            windEvent = scriptable as EventWindowScriptable;
            bool breakEarly = Random.Range(0, 3) > 1;
            if (breakEarly)
            {
                BreakWindows(room);
            }
            return true;
        }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room)
        {
            return true;
        }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room)
        {
            return true;
        }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
        {
            if (!room.Holder.Find("FreezingArea(Clone)"))
            {
                BreakWindows(room);
                Soundsystem.PlaySound(windEvent.windowBreakingClip, room.transform.position);
            }
            return true;
        }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room)
        {
            return true;
        }
        //First time completing room
        public override bool FirstExit(CarriageClass room)
        {
            return true;
        }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room)
        {
            return true;
        }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room)
        {
            return true;
        }
        //Getting far away from the room
        public override bool Recede(CarriageClass room)
        {
            return true;
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            if (spawnedFreezingArea) { Destroy(spawnedFreezingArea); }
            PlrRefs.inst.PlayerStatusEffects.ManageFrostbiteCauses("Windows", true);
            Destroy(this);
            return true;
        }

        private void BreakWindows(CarriageClass room)
        {
            spawnedFreezingArea = Instantiate(scriptable.SpawnablePrefab);
            spawnedFreezingArea.transform.parent = room.Holder;
            BoxCollider _roomCol = room.GetComponent<BoxCollider>();
            spawnedFreezingArea.transform.localScale = new Vector3(_roomCol.size.x, _roomCol.size.y, _roomCol.size.z - 2);
            spawnedFreezingArea.transform.position = room.transform.position + _roomCol.center;

            List<MeshFilter> possibleWindows = room.transform.Find("Gameplay").Find("Windows").GetComponentsInChildren<MeshFilter>().ToList();

            //break windows visually
            foreach (MeshFilter window in possibleWindows)
            {
                window.mesh = windEvent.possibleWindowMeshes[Random.Range(0, windEvent.possibleWindowMeshes.Length)];
                window.gameObject.layer = 11;
            }
        }
    }
}
