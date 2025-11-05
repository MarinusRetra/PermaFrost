using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/AllEars")]
    public class AllEarsEvent : EventClass
    {
        [SerializeField] private GameObject _allEarsPrefab;
        private GameObject _spawnedAllEars;
        public override bool Entered()
        {
            _spawnedAllEars = Instantiate(_allEarsPrefab);
            return true;
        }
        public override bool Exited()
        {
            Destroy(_spawnedAllEars);
            return true;
        }
        public override bool Triggered()
        {
            return true;
        }
    }
}
