using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;

public class Generation : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private List<GameObject> _rooms;
    [SerializeField] private GameObject _endRoom;

    [SerializeField] private NavMeshSurface _meshSurface;

    private List<GameObject> _initializedRooms = new List<GameObject>();

    private int _amountOfRooms = 5;

    void Start()
    {
        GenerateRooms();
        StartCoroutine(GenerateNavmesh());
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
        for (int i = 0; i < _amountOfRooms; i++)
        {
            GameObject randomRoom = Instantiate(_rooms[Random.Range(0, _rooms.Count)]);

            if (i > 0)
            {
                GameObject previousRoom = _initializedRooms[i - 1];
                PositionGeneratedRoom(randomRoom, previousRoom);
            }

            _initializedRooms.Add(randomRoom);

            randomRoom.transform.parent = transform;
        }

        GameObject endRoom = Instantiate(_endRoom);
        PositionGeneratedRoom(endRoom, _initializedRooms[_initializedRooms.Count - 1]);
    }

    private IEnumerator GenerateNavmesh()
    {
        yield return new WaitForEndOfFrame();
        _meshSurface.BuildNavMesh();
    }
}
