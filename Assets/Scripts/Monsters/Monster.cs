using UnityEngine;

namespace Gameplay
{
    public class Monster : MonoBehaviour
    {
        //This script can be used for any shared things between monsters
        public Transform CurrentRoom;

        public virtual void Deaggro() { }

        public virtual void Aggro(Vector3 location) { }

        public virtual void Aggro() { }

        //update this for any monster that does effects and such
        public virtual void DestroyMonster() { Destroy(gameObject); }
    }
}
