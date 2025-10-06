using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private List<GameObject> _rooms;
    private int _amountOfRooms = 5;
    private List<GameObject> _initializedRooms = new List<GameObject>();

    void Start()
    {
        GenerateRooms();
    }

    void GenerateRooms()
    {
        for (int i = 0; i < _amountOfRooms; i++)
        {
            // Instantiate a random room
            GameObject randomRoom = Instantiate(_rooms[Random.Range(0, _rooms.Count)]);
            CarriageClass currentCarriage = randomRoom.GetComponent<CarriageClass>();

            if (i > 0)
            {
                // Get previous room and its exit point
                GameObject previousRoom = _initializedRooms[i - 1];
                CarriageClass prevCarriage = previousRoom.GetComponent<CarriageClass>();

                // Align entry of new room to exit of previous one
                Transform entry = currentCarriage.entryPoint;
                Transform exit = prevCarriage.exitPoint;

                // Calculate the offset from the entry to the room’s root
                Vector3 entryOffset = randomRoom.transform.position - entry.position;

                // Match position and rotation
                randomRoom.transform.position = exit.position + entryOffset;
                randomRoom.transform.rotation = exit.rotation * Quaternion.Inverse(entry.rotation);
            }

            _initializedRooms.Add(randomRoom);
        }
    }
}
