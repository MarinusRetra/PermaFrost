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
        public override bool Entered(GameObject room)
        {
            if (!room.transform.Find("FreezingArea(Clone)"))
            {
                BreakWindows(room);

                Soundsystem.PlaySound(_windowBreakingClip, room.transform.position);
            }
            return true;
        }
        public override bool Exited(GameObject room)
        {
            //DO NOT DESPAWN
            return true;
        }
        public override bool Triggered(GameObject room) { return true; }

        public override bool Generated(GameObject room) 
        {
            bool doEarly = Random.Range(0, 3) > 1;
            if (doEarly)
            {
                BreakWindows(room);
            }
            return true; 
        }

        private void BreakWindows(GameObject room)
        {
            GameObject _freeze = Instantiate(_freezingPrefab);
            _freeze.transform.parent = room.transform;
            _freeze.transform.localScale = new Vector3(3, 1, 1.75f);
            _freeze.transform.position = room.transform.position;

            List<Transform> possibleWindows = room.transform.Find("Windows").GetComponentsInChildren<Transform>().ToList();
            possibleWindows.RemoveAt(0);

            //break windows visually
            foreach (Transform window in possibleWindows)
            {
                window.GetComponent<MeshFilter>().mesh = _possibleWindowMeshes[Random.Range(0, _possibleWindowMeshes.Length)];
                window.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
