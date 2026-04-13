using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/ShadowmanEvent")]
    public class ShadowmanEvent : EventClass
    {
        [SerializeField]
        private GameObject _shadowmanPrefab;
        //public override bool Entered(CarriageClass room)
        //{
        //    GameObject _shadowman = Instantiate(_shadowmanPrefab,new Vector3(-300,-300,-300),new Quaternion(0,0,0,0));
        //    _shadowman.GetComponent<Monster>().CurrentRoom = room.transform;
        //    _shadowman.transform.parent = room.Holder;
        //    return true;
        //}
        //public override bool Exited(CarriageClass room) {return true;}
        //public override bool Generated(CarriageClass room) { return true; }
        //public override bool CallForDeletion(CarriageClass room) { return true; }
    }
}
