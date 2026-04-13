using Gameplay;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Generation : MonoBehaviour
{
    public static Generation mainInstance;
    [Header("Rooms")]
    public RoomTypeScriptable Rooms;
    public RoomTypeScriptable PlaceholderRooms;

    [SerializeField] private NavMeshSurface _meshSurface;

    public List<GameObject> _initializedRooms = new List<GameObject>();
    public List<CarriageClass> _initializedCarriages = new List<CarriageClass>();
    public int AmountOfRooms = 15;
    public GameObject player;

    public bool FastLoading = false;

    private int currentHeightValue = 0;

    public bool IsGenerating = false;

    public int RoomApproachSize = 2;

    void Start()
    {
        mainInstance = this;
        StartCoroutine(GenerateRooms());
    }

    void PositionGeneratedRoom(GameObject room, GameObject previousRoom)
    {
        Transform exit = null;
        CarriageClass prevCarriage = null;
        if (previousRoom)
        {
            prevCarriage = previousRoom.GetComponent<CarriageClass>();
            exit = prevCarriage.ExitPoint;
            prevCarriage.generationClass = this;
        }
        else
        {
            exit = transform;
        }
        CarriageClass currentCarriage = room.GetComponent<CarriageClass>();

        Transform entry = currentCarriage.EntryPoint;


        Vector3 entryOffset = room.transform.position - entry.position;

        room.transform.position = exit.position + entryOffset;
        room.transform.rotation = exit.rotation * Quaternion.Inverse(entry.rotation);

        currentCarriage.generationClass = this;

        currentCarriage.SpawnItems();
    }

    private string prevRoomClassName;
    IEnumerator GenerateRooms()
    {
        IsGenerating = true;
        currentHeightValue = 0;
        yield return new WaitForSeconds(0.3f * (FastLoading ? 0 : 1));
        GameObject startRoomPref = null;
        if (Rooms.HasStartRoom)
        {
            startRoomPref = Rooms.RoomTypeStartRoom;
        }
        else
        {
            startRoomPref = PlaceholderRooms.RoomTypeStartRoom;
        }
        GameObject startRoom = Instantiate(startRoomPref, transform.position, transform.rotation);
        _initializedRooms.Add(startRoom);
        _initializedCarriages.Add(startRoom.GetComponent<CarriageClass>());


        if (startRoom.GetComponent<CarriageClass>().PlayerSpawnPoint && player)
        {
            player.transform.position = _initializedCarriages[0].PlayerSpawnPoint.transform.position;
            player.SetActive(true);
        }
        yield return new WaitForSeconds(0.1f * (FastLoading ? 0 : 1));

        allTotalPossibleRooms = new List<RoomClass>(Rooms.AllRoomsInType);

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
        _initializedCarriages.Add(endRoom.GetComponent<CarriageClass>());
        StartCoroutine(GenerateNavmesh());

        for(int i = 0; i < _initializedRooms.Count; i++)
        {
            if (i <= RoomApproachSize)
            {
                _initializedCarriages[i].OnApproach(true);
            }
            else
            {
                _initializedCarriages[i].OnRecede(true);
            }
        }

        if(Rooms.HasStartRoom == false)
        {
            Destroy(_initializedRooms[0].gameObject);
        }
        IsGenerating = false;
    }

#if UNITY_EDITOR

    public void RegenerateRooms()
    {
        if (IsGenerating) return;
        for(int i = 0;i < _initializedRooms.Count;i++)
        {
            _initializedCarriages[i].DespawnItems();
            Destroy(_initializedRooms[i]);
        }
        _initializedRooms = new List<GameObject>();
        _initializedCarriages = new List<CarriageClass>();
        prevRoomCarriage = null;
        prevRoomClassName = null;
        StartCoroutine(GenerateRooms());
    }
#endif

    private CarriageClass prevRoomCarriage;
    private void GiveRoomEvents(CarriageClass room, RoomClass roomClass)
    {
        List<EventClass> allowedEvents = new List<EventClass>(roomClass.AllowedEvents);

        if (prevRoomCarriage != null && !Rooms.AllowEventDupes)
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
                _chosenEvent.Generate(room);
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
            GameObject previousRoom = null;
            if(_initializedRooms.Count != 0)
            {
                previousRoom = _initializedRooms[index];
            }
            PositionGeneratedRoom(randomRoom, previousRoom);

            randomRoom.name = "Room" + index + selectedroom.RoomName;

            CarriageClass randomCarriage = randomRoom.GetComponent<CarriageClass>();
            randomCarriage.previousCarriage = previousRoom?.GetComponent<CarriageClass>();
            randomCarriage.roomIndex = index + 1;
            if (previousRoom) { previousRoom.GetComponent<CarriageClass>().nextCarriage = randomCarriage; }

            _initializedRooms.Add(randomRoom);
            _initializedCarriages.Add(randomCarriage);
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

    private int currentRoomIndex = -999;
    public void EnterRoom(int index)
    {
        if(currentRoomIndex == index) { return; }
        int acceptableI = -RoomApproachSize;
        if (index >= RoomApproachSize + 1)
        {
            _initializedRooms[index - RoomApproachSize - 1].GetComponent<CarriageClass>().OnRecede();
            if(index - currentRoomIndex > 1 && index >= RoomApproachSize + 2)
            {
                _initializedRooms[index - RoomApproachSize - 2].GetComponent<CarriageClass>().OnRecede();
            }
        }
        else
        {
            acceptableI = 1;
        }
        for (int i = acceptableI; i < RoomApproachSize + 1; i++)
        {
            if(i + index > AmountOfRooms) { continue; }
            _initializedRooms[index + i]?.GetComponent<CarriageClass>().OnApproach();
        }

        currentRoomIndex = index;
        if(index + RoomApproachSize + 1 <= AmountOfRooms)
        {
            _initializedRooms[index + RoomApproachSize + 1]?.GetComponent<CarriageClass>().OnRecede();
        }
    }
}
