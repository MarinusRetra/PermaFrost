using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;
using Gameplay;

public class Generation : MonoBehaviour
{
    [Header("Rooms")]
    public RoomTypeScriptable Rooms;

    [SerializeField] private NavMeshSurface _meshSurface;

    [SerializeField] private List<GameObject> _initializedRooms = new List<GameObject>();
    public int AmountOfRooms = 15;
    public GameObject player;

    public bool FastLoading = false;

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
        yield return new WaitForSeconds(0.3f * (FastLoading ? 0 : 1));
        GameObject startRoom = Instantiate(Rooms.RoomTypeStartRoom, transform.position,transform.rotation);
        _initializedRooms.Add(startRoom);

        
        if (startRoom.GetComponent<CarriageClass>().PlayerSpawnPoint)
        {
            player.GetComponent<Transform>().position = startRoom.GetComponent<CarriageClass>().PlayerSpawnPoint.transform.position;
            player.SetActive(true);
        }
        yield return new WaitForSeconds(0.1f * (FastLoading ? 0 : 1));
        GameObject previousRandomRoom = null;
        for (int i = 0; i < AmountOfRooms; i++)
        {

            //SpawnWeightedRoom(i);
            GameObject selectedRoom = Rooms.AllRoomsInType[Random.Range(0, Rooms.AllRoomsInType.Length)];

            //prevent dupe rooms
            if (selectedRoom == previousRandomRoom && !Rooms.AllowDupes)
            {
                for (int j = 0; j < 5; j++)
                {
                    selectedRoom = Rooms.AllRoomsInType[Random.Range(0, Rooms.AllRoomsInType.Length)];
                    if (selectedRoom != previousRandomRoom) { j = 100; }
                }
            }
            previousRandomRoom = selectedRoom;
            GameObject randomRoom = Instantiate(selectedRoom);
            GameObject previousRoom = _initializedRooms[i];
            PositionGeneratedRoom(randomRoom, previousRoom);

            CarriageClass randomCarriage = randomRoom.GetComponent<CarriageClass>();
            randomCarriage.previousCarriage = previousRoom.GetComponent<CarriageClass>();

            _initializedRooms.Add(randomRoom);
            randomRoom.transform.parent = transform;
            _meshSurface.UpdateNavMesh(_meshSurface.navMeshData);
            yield return new WaitForSeconds(0.3f * (FastLoading ? 0 : 1));
        }

        GameObject endRoom = Instantiate(Rooms.RoomTypeEndRoom);
        PositionGeneratedRoom(endRoom, _initializedRooms[_initializedRooms.Count - 1]);
        _initializedRooms.Add(endRoom);
        StartCoroutine(GenerateNavmesh());
    }

    private void SpawnWeightedRoom(int index)
    {
        RoomClass selectedroom = new RoomClass();
        int totalWeight = 0;
        for(int i = 0;i < Rooms.AllRoomsSquaredInType.Length; i++)
        {
            totalWeight += Rooms.AllRoomsSquaredInType[i].Weight;
            print("Current weight: " + totalWeight + ". Weight added this go: " + Rooms.AllRoomsSquaredInType[i].Weight);
        }
        int randomChosenWeight = Random.Range(1, totalWeight + 1);
        int roomCheckers = 0;
        for (int i = 0; i < Rooms.AllRoomsSquaredInType.Length; i++)
        {
            roomCheckers += Rooms.AllRoomsSquaredInType[i].Weight;
            print("Current weight: " + roomCheckers + ". Weight added this go: " + Rooms.AllRoomsSquaredInType[i].Weight + ". We are looking for: " + randomChosenWeight);
            if (roomCheckers > randomChosenWeight || roomCheckers == totalWeight)
            {
                selectedroom = Rooms.AllRoomsSquaredInType[i];
                print("Chosen room!" + Rooms.AllRoomsSquaredInType[i].RoomName + " At a value of " + randomChosenWeight + " Who has a weight of " + Rooms.AllRoomsSquaredInType[i].Weight);
                break;
            }
        }
        if (selectedroom.Room == null)
        {
            Debug.LogError("Room doesnt have a room");
        }
        else
        {

            GameObject randomRoom = Instantiate(selectedroom.Room);
            GameObject previousRoom = _initializedRooms[index];
            PositionGeneratedRoom(randomRoom, previousRoom);

            CarriageClass randomCarriage = randomRoom.GetComponent<CarriageClass>();
            randomCarriage.previousCarriage = previousRoom.GetComponent<CarriageClass>();

            _initializedRooms.Add(randomRoom);
            randomRoom.transform.parent = transform;
            _meshSurface.UpdateNavMesh(_meshSurface.navMeshData);
        }

    }

    private IEnumerator GenerateNavmesh()
    {
        yield return new WaitForEndOfFrame();
        _meshSurface.UpdateNavMesh(_meshSurface.navMeshData);
    }
}
