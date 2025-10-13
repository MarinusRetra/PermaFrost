using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private List<GameObject> _rooms;
    [SerializeField] private GameObject _startRoom;
    [SerializeField] private GameObject _endRoom;

    private List<GameObject> _initializedRooms = new List<GameObject>();

    private int _amountOfRooms = 5;

    void Start()
    {
        GenerateRooms();
    }

    void PositionGeneratedRoom(GameObject room, GameObject previousRoom)
    {
        CarriageClass currentCarriage = room.GetComponent<CarriageClass>();
        CarriageClass prevCarriage = previousRoom.GetComponent<CarriageClass>();

        Transform entry = currentCarriage.entryPoint;
        Transform exit = prevCarriage.exitPoint;

        Vector3 entryOffset = room.transform.position - entry.position;

        room.transform.position = exit.position + entryOffset;
        room.transform.rotation = exit.rotation * Quaternion.Inverse(entry.rotation);
    }

    void GenerateRooms()
    {
        GameObject startRoom = Instantiate(_startRoom);
        _initializedRooms.Add(startRoom);

        for (int i = 0; i < _amountOfRooms; i++)
        {
            GameObject randomRoom = Instantiate(_rooms[Random.Range(0, _rooms.Count)]);
            GameObject previousRoom = _initializedRooms[i];
            PositionGeneratedRoom(randomRoom, previousRoom);

            _initializedRooms.Add(randomRoom);
        }

        GameObject endRoom = Instantiate(_endRoom);
        PositionGeneratedRoom(endRoom, _initializedRooms[_initializedRooms.Count - 1]);
    }
}
