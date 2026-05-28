using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class EventRefs : Singleton<EventRefs>
    {
        public EventClass[] EventClasses;
        public EventClassScriptable[] EventClassScriptables;

        private void Start()
        {
            IEnumerable<EventClass> sortedEventsClasses = EventClasses.OrderBy(x => x.id);
            EventClasses = sortedEventsClasses.ToArray();
            IEnumerable<EventClassScriptable> sortedEventsClassScriptables = EventClassScriptables.OrderBy(x => x.id);
            EventClassScriptables = sortedEventsClassScriptables.ToArray();
        }

        public EventClass GetClassFromScriptable(EventClassScriptable scriptable)
        {
            return EventClasses[scriptable.id];
        }
    }
}
