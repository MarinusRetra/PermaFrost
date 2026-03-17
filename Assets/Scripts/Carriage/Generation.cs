using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;
using Gameplay;

public class Generation : MonoBehaviour
{
    public static Generation mainInstance;
    [Header("Rooms")]
    public RoomTypeScriptable Rooms;

    [SerializeField] private NavMeshSurface _meshSurface;

    [SerializeField] private List<GameObject> _initializedRooms = new List<GameObject>();
    public int AmountOfRooms = 15;
    public GameObject player;

    public bool FastLoading = false;

    private int currentHeightValue = 0;

    public bool IsGenerating = false;

    void Start()
    {
        mainInstance = this;
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

    private string prevRoomClassName;
    IEnumerator GenerateRooms()
    {
        IsGenerating = true;
        currentHeightValue = 0;
        yield return new WaitForSeconds(0.3f * (FastLoading ? 0 : 1));
        GameObject startRoom = Instantiate(Rooms.RoomTypeStartRoom, transform.position,transform.rotation);
        _initializedRooms.Add(startRoom);

        
        if (startRoom.GetComponent<CarriageClass>().PlayerSpawnPoint)
        {
            player.transform.position = startRoom.GetComponent<CarriageClass>().PlayerSpawnPoint.transform.position;
            player.SetActive(true);
        }
        yield return new WaitForSeconds(0.1f * (FastLoading ? 0 : 1));

        allTotalPossibleRooms = new List<RoomClass>(Rooms.AllRoomsInType);

        GameObject previousRandomRoom = null;
        for (int i = 0; i < AmountOfRooms; i++)
        {

            SpawnWeightedRoom(i);
            yield return new WaitForSeconds(0.3f * (FastLoading ? 0.1f : 1));
            for (int j = 0; j < 10; j++)
            {
                if (generatingRoom)
                {
                    yield return new WaitForSeconds(0.3f * (FastLoading ? 0.1f : 1));
                }
            }
        }

        GameObject endRoom = Instantiate(Rooms.RoomTypeEndRoom);
        PositionGeneratedRoom(endRoom, _initializedRooms[_initializedRooms.Count - 1]);
        _initializedRooms.Add(endRoom);
        StartCoroutine(GenerateNavmesh());
        IsGenerating = false;
    }

#if UNITY_EDITOR

    public void RegenerateRooms()
    {
        if (IsGenerating) return;
        for(int i = 0;i < _initializedRooms.Count;i++)
        {
            _initializedRooms[i].GetComponent<CarriageClass>().DespawnItems();
            Destroy(_initializedRooms[i]);
        }
        _initializedRooms = new List<GameObject>();
        prevRoomCarriage = null;
        prevRoomClassName = null;
        StartCoroutine(GenerateRooms());
    }
#endif

    private CarriageClass prevRoomCarriage;
    private void GiveRoomEvents(CarriageClass room, RoomClass roomClass)
    {
        List<EventClass> allowedEvents = new List<EventClass>(roomClass.AllowedEvents);

        if (prevRoomCarriage != null)
        {
            List<EventClass> prevRoomEvents = prevRoomCarriage._selectedEventClasses;
            for (int i = 0; i <  prevRoomEvents.Count; i++)
            {
                if (allowedEvents.Contains(prevRoomEvents[i]))
                {
                    allowedEvents.Remove(prevRoomEvents[i]);
                }
            }
        }

        if (allowedEvents.Count > 0 && roomClass.AmountOfEventsMax > 0)
        {
            int count = Mathf.Min(roomClass.AmountOfEventsMax, allowedEvents.Count);
            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, allowedEvents.Count);
                int index = Mathf.Min(randomIndex, allowedEvents.Count);
                if(allowedEvents.Count == 0) { prevRoomCarriage = room; return; }
                EventClass _chosenEvent = allowedEvents[index];
                room._selectedEventClasses.Add(_chosenEvent);
                allowedEvents.RemoveAt(index);
                _chosenEvent.Generated(room);
                if (_chosenEvent is WindowEvent || _chosenEvent is HotDudeEvent)
                {
                    for (int j = 0; j < allowedEvents.Count; j++)
                    {
                        if (allowedEvents[j] is HotDudeEvent || allowedEvents[j] is WindowEvent)
                        {
                            allowedEvents.RemoveAt(j);
                        }
                    }
                }
            }
        }

        prevRoomCarriage = room;

    }
    bool generatingRoom = false;
    List<RoomClass> allTotalPossibleRooms = new List<RoomClass>();
    private void SpawnWeightedRoom(int index)
    {
        generatingRoom = true;
        List<RoomClass> allCurrentPossibleRooms = new List<RoomClass>(allTotalPossibleRooms);
        if(index > 0 && !Rooms.AllowDupes)
        {
            for (int i = 0; i < allCurrentPossibleRooms.Count; i++)
            {
                if (allCurrentPossibleRooms[i].RoomName == prevRoomClassName)
                {
                    allCurrentPossibleRooms.Remove(allCurrentPossibleRooms[i]);
                    break;
                }
            }
        }
        RoomClass selectedroom = new RoomClass();

        for (int i = 0; i < Rooms.GuarenteedRooms.Length; i++)
        {
            if (Rooms.GuarenteedRooms[i].guarenteedIndex - 1 == index)
            {
                selectedroom = Rooms.GuarenteedRooms[i];
            }
        }
        if (!selectedroom.Room)
        {
            for (int i = 0; i < 5; i++)
            {
                selectedroom = CalculateRoomWeight(allCurrentPossibleRooms);

                if (selectedroom.HeightValue < 0 && currentHeightValue == 0)
                {
                    allCurrentPossibleRooms.Remove(selectedroom);
                    selectedroom = CalculateRoomWeight(allCurrentPossibleRooms);
                    continue;
                }
                i = 5;
                currentHeightValue += selectedroom.HeightValue;
            }
        }

        
        if (selectedroom.Room == null)
        {
            Debug.LogError("Room doesnt have a room");
        }
        else
        {
            GameObject randomRoom = Instantiate(selectedroom.Room);
            GiveRoomEvents(randomRoom.GetComponent<CarriageClass>(), selectedroom);
            GameObject previousRoom = _initializedRooms[index];
            PositionGeneratedRoom(randomRoom, previousRoom);

            randomRoom.name = "Room" + index + selectedroom.RoomName;

            CarriageClass randomCarriage = randomRoom.GetComponent<CarriageClass>();
            randomCarriage.previousCarriage = previousRoom.GetComponent<CarriageClass>();
            previousRoom.GetComponent<CarriageClass>().nextCarriage = randomCarriage;

            _initializedRooms.Add(randomRoom);
            randomRoom.transform.parent = transform;
            prevRoomClassName = selectedroom.RoomName;
            _meshSurface.UpdateNavMesh(_meshSurface.navMeshData);

            if (selectedroom.onlySpawnOnce == true)
            {
                allTotalPossibleRooms.Remove(selectedroom);
            }
        }
        generatingRoom = false;
    }

    private RoomClass CalculateRoomWeight(List<RoomClass> rooms)
    {
        int totalWeight = 0;
        for (int i = 0; i < rooms.Count; i++)
        {
            totalWeight += rooms[i].Weight;
        }
        int randomChosenWeight = Random.Range(1, totalWeight + 1);
        int roomCheckers = 0;
        for (int i = 0; i < rooms.Count; i++)
        {
            roomCheckers += rooms[i].Weight;
            if (roomCheckers > randomChosenWeight || roomCheckers == totalWeight)
            {
                return rooms[i];
            }
        }
        return new RoomClass();
    }

    private IEnumerator GenerateNavmesh()
    {
        yield return new WaitForEndOfFrame();
        _meshSurface.UpdateNavMesh(_meshSurface.navMeshData);
    }
}
