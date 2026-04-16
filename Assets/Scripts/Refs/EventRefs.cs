using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class EventRefs : Singleton<EventRefs>
    {
        public EventClass[] EventClasses;

        private void Start()
        {
            IEnumerable<EventClass> sortedEventsClasses = EventClasses.OrderBy(x => x.id);
            EventClasses = sortedEventsClasses.ToArray();
        }

        public EventClass GetClassFromScriptable(EventClassScriptable scriptable)
        {
            return EventClasses[scriptable.id];
        }
    }
}
