using UnityEngine;

namespace Gameplay
{
    public class Monster : MonoBehaviour
    {
        //This script can be used for any shared things between monsters
        //Currently that is if the monster attacks the player

        public virtual void Deaggro() { }

        public virtual void Aggro(Vector3 location) { }
    }
}
