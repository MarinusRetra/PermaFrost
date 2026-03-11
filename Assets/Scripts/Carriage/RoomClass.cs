using System;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public struct RoomClass
    {
        public GameObject Room;
        public int Weight;
        public int AmountOfEventsMax;
        public EventClass[] AllowedEvents;
        public string RoomName;
        public bool onlySpawnOnce;
    }
}
