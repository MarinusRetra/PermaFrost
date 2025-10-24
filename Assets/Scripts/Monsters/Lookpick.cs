using UnityEngine;

namespace Gameplay
{
    public class Lookpick : Monster
    {
        public override void Aggro(Vector3 location)
        {
            //turn on mesh
            //location should be the locks location, so tp to spot below
            //slowly move to player
            //freeze or move back when looked at (make functionality for both)
        }

        public override void Deaggro()
        {
            //turn off mesh
            //turn off collider
        }
    }
}
