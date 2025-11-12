using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/BrokenWindowEvent")]
    public class WindowEvent : EventClass
    {
        [SerializeField]
        private GameObject _freezingPrefab;
        public override bool Entered(GameObject room)
        {
            GameObject _freeze = Instantiate(_freezingPrefab);
            _freeze.transform.parent = room.transform;
            _freeze.transform.localScale = new Vector3(3,1,0.85f);
            _freeze.transform.position = room.transform.position;

            //break windows visually
            return true;
        }
        public override bool Exited(GameObject room)
        {
            //DO NOT DESPAWN
            return true;
        }
        public override bool Triggered(GameObject room)
        {
            return true;
        }
    }
}
