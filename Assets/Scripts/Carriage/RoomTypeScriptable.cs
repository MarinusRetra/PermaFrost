using System;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "RoomType")]
    [Serializable]
    public class RoomTypeScriptable : ScriptableObject
    {
        public string RoomtypeName;

        public GameObject RoomTypeStartRoom;
        public GameObject RoomTypeEndRoom;

        public GameObject[] AllRoomsInType;
        public RoomClass[] AllRoomsSquaredInType;

        public bool AllowDupes = false;
    }
}
