using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class RoomGroupParent : MonoBehaviour
    {
        public GameObject StartingRoom;
        public CarriageClass StartingCarriage;
        public List<GameObject> _initializedRooms = new List<GameObject>();
        public List<CarriageClass> _initializedCarriages = new List<CarriageClass>();
        public GameObject EndingRoom;

        public int AmountOfRooms = -999;
    }
}
