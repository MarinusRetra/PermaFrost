using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/BrokenWindowEvent")]
    public class WindowEvent : EventClass
    {
        public override bool Entered(GameObject room)
        {
            return true;
        }
        public override bool Exited(GameObject room)
        {
            return true;
        }
        public override bool Triggered(GameObject room)
        {
            return true;
        }
    }
}
