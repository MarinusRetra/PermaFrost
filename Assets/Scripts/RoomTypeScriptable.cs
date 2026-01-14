using System;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "RoomType")]
    [Serializable]
    public class RoomTypeScriptable : ScriptableObject
    {
        public string RoomtypeName;
        public GameObject[] AllRoomsInType;
    }
}
