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

    [SerializeField] private List<GameObject> _initializedRooms = new List<GameObject>();
    private int _amountOfRooms = 15;
    public GameObject player;

    void Start()
    {
        StartCoroutine(GenerateRooms());
    }

    void PositionGeneratedRoom(GameObject room, GameObject previousRoom)
    {
        CarriageClass currentCarriage = room.GetComponent<CarriageClass>();
        CarriageClass prevCarriage = previousRoom.GetComponent<CarriageClass>();

        Transform entry = currentCarriage.EntryPoint;
        Transform exit = prevCarriage.ExitPoint;

        Vector3 entryOffset = room.transform.position - entry.position;

        room.transform.position = exit.position + entryOffset;
        room.transform.rotation = exit.rotation * Quaternion.Inverse(entry.rotation);

        currentCarriage.generationClass = this;
        prevCarriage.generationClass = this;

        currentCarriage.SpawnItems();
    }

    IEnumerator GenerateRooms()
    {
        yield return new WaitForSeconds(0.3f);
        GameObject startRoom = Instantiate(_startRoom);
        _initializedRooms.Add(startRoom);

        if (startRoom.GetComponent<CarriageClass>().SpawnPoint)
        {
            player.GetComponent<Transform>().position = startRoom.GetComponent<CarriageClass>().SpawnPoint.transform.position;
            player.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        for (int i = 0; i < _amountOfRooms; i++)
        {
            GameObject randomRoom = Instantiate(_rooms[Random.Range(0, _rooms.Count)]);
            GameObject previousRoom = _initializedRooms[i];
            PositionGeneratedRoom(randomRoom, previousRoom);

            _initializedRooms.Add(randomRoom);
            randomRoom.transform.parent = transform;
            yield return new WaitForSeconds(0.3f);
        }

        GameObject endRoom = Instantiate(_endRoom);
        PositionGeneratedRoom(endRoom, _initializedRooms[_initializedRooms.Count - 1]);
        _initializedRooms.Add(endRoom);
        yield return new WaitForSeconds(10f);
        StartCoroutine(GenerateNavmesh());
    }

    private IEnumerator GenerateNavmesh()
    {
        yield return new WaitForEndOfFrame();
        _meshSurface.BuildNavMesh();
    }
}
