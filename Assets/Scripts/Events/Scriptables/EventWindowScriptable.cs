using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/Specific/WindowEvent")]
    public class EventWindowScriptable : EventClassScriptable
    {
        public Mesh[] possibleWindowMeshes;
        public AudioClip windowBreakingClip;
    }
}
