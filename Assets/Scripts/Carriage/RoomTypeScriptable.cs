using System;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "RoomType")]
    [Serializable]
    public class RoomTypeScriptable : ScriptableObject
    {
        public string RoomtypeName;

        public bool HasStartRoom = true;
        public GameObject RoomTypeStartRoom;
        public GameObject RoomTypeEndRoom;

        public RoomClass[] AllRoomsInType;

        public RoomClass[] GuarenteedRooms;

        public bool AllowDupes = false;
    }
}
