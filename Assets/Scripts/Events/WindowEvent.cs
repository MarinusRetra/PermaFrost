using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/BrokenWindowEvent")]
    public class WindowEvent : EventClass
    {
        [SerializeField]
        private GameObject _freezingPrefab;

        [SerializeField] private Mesh[] _possibleWindowMeshes;

        [SerializeField] private AudioClip _windowBreakingClip;
        //Variables

        //When room spawns in
        public override bool Generate(CarriageClass room)
        {
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

                Soundsystem.PlaySound(_windowBreakingClip, room.transform.position);
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
            Destroy(room.Holder.Find("FreezingArea(Clone)")?.gameObject);
            PlrRefs.inst.PlayerStatusEffects.ManageFrostbiteCauses("Windows", true);
            return true;
        }

        private void BreakWindows(CarriageClass room)
        {
            GameObject _freeze = Instantiate(_freezingPrefab);
            _freeze.transform.parent = room.Holder;
            BoxCollider _roomCol = room.GetComponent<BoxCollider>();
            _freeze.transform.localScale = new Vector3(_roomCol.size.x, _roomCol.size.y, _roomCol.size.z - 2);
            _freeze.transform.position = room.transform.position + room.GetComponent<BoxCollider>().center;

            List<Transform> possibleWindows = room.transform.Find("Gameplay").Find("Windows").GetComponentsInChildren<Transform>().ToList();
            possibleWindows.RemoveAt(0);

            //break windows visually
            foreach (Transform window in possibleWindows)
            {
                window.GetComponent<MeshFilter>().mesh = _possibleWindowMeshes[Random.Range(0, _possibleWindowMeshes.Length)];
                window.gameObject.layer = 11;
            }
        }
    }
}
