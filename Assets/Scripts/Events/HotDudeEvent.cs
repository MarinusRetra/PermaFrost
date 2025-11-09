using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/HotDudeEvent")]
    public class HotDudeEvent : EventClass
    {
        [SerializeField]
        private GameObject _hotDudePrefab;
        public override bool Entered(GameObject room)
        {
            return true;
        }
        public override bool Exited(GameObject room)
        {
            if (!room.transform.Find("Monsters").Find("HotDude(Clone)")) return false;
            room.transform.Find("Monsters").Find("HotDude(Clone)").GetComponent<Monster>().DestroyMonster();
            return true;
        }
        public override bool Triggered(GameObject room)
        {
            GameObject _hotDude = Instantiate(_hotDudePrefab);
            _hotDude.transform.parent = room.transform.Find("Monsters");
            Transform _entry = room.transform.Find("Entry");
            _hotDude.transform.position = new Vector3(_entry.position.x, _entry.position.y + 0.1f, _entry.position.z + 0.5f);
            _hotDude.GetComponent<NavMeshAgent>().enabled = true;
            return true;
        }
    }
}
