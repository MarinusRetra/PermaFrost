using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/ShadowmanEvent")]
    public class ShadowmanEvent : EventClass
    {
        [SerializeField]
        private GameObject _shadowmanPrefab;
        public override bool Entered(GameObject room)
        {
            GameObject _shadowman = Instantiate(_shadowmanPrefab);
            _shadowman.GetComponent<Monster>().CurrentRoom = room.transform;
            _shadowman.transform.parent = room.transform.Find("Monsters");
            return true;
        }
        public override bool Exited(GameObject room)
        {
            if (room.transform.Find("Monsters").Find("Shadowman(Clone)"))
            {
                room.transform.Find("Monsters").Find("Shadowman(Clone)").GetComponent<Monster>().DestroyMonster();
            }
            return true;
        }
        public override bool Triggered(GameObject room)
        {
            return true;
        }
    }
}
