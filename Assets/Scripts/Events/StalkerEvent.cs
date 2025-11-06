using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/StalkerEvent")]
    public class StalkerEvent : EventClass
    {
        [SerializeField]
        private GameObject _stalkerPrefab;
        public override bool Entered(GameObject room)
        {
            GameObject _stalker = Instantiate(_stalkerPrefab);
            _stalker.GetComponent<Monster>().CurrentRoom = room.transform;
            _stalker.transform.parent = room.transform.Find("Monsters");
            return true;
        }
        public override bool Exited(GameObject room)
        {
            room.transform.Find("Monsters").Find("Stalker(Clone)").GetComponent<Monster>().DestroyMonster();
            return true;
        }
        public override bool Triggered(GameObject room)
        {
            return true;
        }
    }
}
