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
        public override bool Entered(CarriageClass room)
        {
            if (!room.Holder.Find("FreezingArea(Clone)"))
            {
                BreakWindows(room);

                Soundsystem.PlaySound(_windowBreakingClip, room.transform.position);
            }
            return true;
        }
        public override bool Exited(CarriageClass room)
        {
            //DO NOT DESPAWN
            return true;
        }
        public override bool Triggered(CarriageClass room) { return true; }

        public override bool Generated(CarriageClass room) 
        {
            bool doEarly = Random.Range(0, 3) > 1;
            if (doEarly)
            {
                BreakWindows(room);
            }
            return true; 
        }

        public override bool CallForDeletion(CarriageClass room)
        {
            Destroy(room.Holder.Find("FreezingArea(Clone)")?.gameObject);
            PlayerStatusEffects.Instance.ManageFrostbiteCauses("Windows", true);
            return true;
        }

        private void BreakWindows(CarriageClass room)
        {
            GameObject _freeze = Instantiate(_freezingPrefab);
            _freeze.transform.parent = room.Holder;
            BoxCollider _roomCol = room.GetComponent<BoxCollider>();
            _freeze.transform.localScale = new Vector3(_roomCol.size.x,_roomCol.size.y,_roomCol.size.z - 2);
            _freeze.transform.position = room.transform.position + room.GetComponent<BoxCollider>().center;

            List<Transform> possibleWindows = room.transform.Find("Gameplay").Find("Windows").GetComponentsInChildren<Transform>().ToList();
            possibleWindows.RemoveAt(0);

            //break windows visually
            foreach (Transform window in possibleWindows)
            {
                window.GetComponent<MeshFilter>().mesh = _possibleWindowMeshes[Random.Range(0, _possibleWindowMeshes.Length)];
                Destroy(window.GetComponent<Collider>());
            }
        }
    }
}
