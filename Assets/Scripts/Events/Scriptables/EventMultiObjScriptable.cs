using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/MultiObj")]
    public class EventMultiObjScriptable : EventClassScriptable
    {
        public GameObject[] otherPrefabs;
    }
}
