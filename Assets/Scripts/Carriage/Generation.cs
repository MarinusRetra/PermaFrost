using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;
using Gameplay;

public class Generation : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private List<GameObject> _rooms;
    [SerializeField] private GameObject _startRoom;
    [SerializeField] private GameObject _endRoom;

    [SerializeField] private NavMeshSurface _meshSurface;

    private List<GameObject> _initializedRooms = new List<GameObject>();
    private int _amountOfRooms = 15;
    public GameObject player;

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

        currentCarriage.SpawnRandomItem();
    }

    void GenerateRooms()
    {
        GameObject startRoom = Instantiate(_startRoom);
        _initializedRooms.Add(startRoom);
        // playerInventory = startRoom.transform.Find("Player").GetComponent<PlayerInventory>();

        if (startRoom.GetComponent<CarriageClass>().spawnPoint)
        {
            player.GetComponent<Transform>().position = startRoom.GetComponent<CarriageClass>().spawnPoint.transform.position;
            player.SetActive(true);
        }

        for (int i = 0; i < _amountOfRooms; i++)
        {
            GameObject randomRoom = Instantiate(_rooms[Random.Range(0, _rooms.Count)]);
            GameObject previousRoom = _initializedRooms[i];
            PositionGeneratedRoom(randomRoom, previousRoom);

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
