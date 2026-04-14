using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/Generic")]
    public class EventClassScriptable : ScriptableObject
    {
        public MonoScript eventClass;
        public GameObject SpawnablePrefab;
        public EventClassScriptable[] removeEvents;
#if UNITY_EDITOR
        [Header("FYI this one specifically is editor only")]
        public bool IncludeInAllInOne = true;
#endif
    }
}
