using UnityEngine;
using UnityEngine.Timeline;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/Animation")]
    public class EventAnimScriptable : EventClassScriptable
    {
        public TimelineAsset animTimeline;
    }
}
