using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/AllEarsEvent")]
    public class AllEarsEvent : EventClass
    {
        [SerializeField] private GameObject _allEarsPrefab;
        private GameObject _spawnedAllEars;
        public override bool Entered(GameObject room)
        {
            _spawnedAllEars = Instantiate(_allEarsPrefab);
            _spawnedAllEars.transform.parent = room.transform.Find("Monsters");
            _spawnedAllEars.GetComponent<Monster>().CurrentRoom = room.transform;
            _spawnedAllEars.transform.localPosition = new Vector3(0, 0, 0);
            _spawnedAllEars.GetComponent<NavMeshAgent>().enabled = true;
            return true;
        }
        public override bool Exited(GameObject room)
        {
            room.transform.Find("Monsters").Find("AllEars(Clone)").GetComponent<Monster>().DestroyMonster();
            return true;
        }
        public override bool Triggered(GameObject room) { return true; }
        public override bool Generated(GameObject room) { return true; }
    }
}
