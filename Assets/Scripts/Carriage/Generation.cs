using Gameplay;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

public class Generation : MonoBehaviour
{
    public static Generation mainInstance;
    [Header("Rooms")]
    public RoomTypeScriptable Rooms;
    public RoomTypeScriptable PlaceholderRooms;

    [SerializeField] private NavMeshSurface _meshSurface;
    [SerializeField] private GameObject roomGroupPrefab;

    //public List<GameObject> _initializedRooms = new List<GameObject>();
    //public List<CarriageClass> _initializedCarriages = new List<CarriageClass>();
    public List<RoomGroupParent> _initializedRoomGroups = new List<RoomGroupParent>();
    public int AmountOfRooms = 15;
    public GameObject player;

    public EventClass[] eventClasses;

    public bool FastLoading = false;

    public bool IsGenerating = false;

    public int RoomApproachSize = 2;

    public Transform DefaultSpawnLocation;

    //Currently spawning stuffs
    private RoomGroupParent currentParent;
    private Vector3 currentSpawnLoc;
    private RoomTypeScriptable currentType;
    private int currentHeightValue = 0;

    void Start()
    {
        mainInstance = this;    
        StartCoroutine(GenerateRooms(Rooms,DefaultSpawnLocation.position,AmountOfRooms));
    }


    private string prevRoomClassName;
    public IEnumerator GenerateRooms(RoomTypeScriptable SpawningType, Vector3 spawnLoc, int RoomAmount,bool tpPlayer = true)
    {
        IsGenerating = true;
        currentHeightValue = 0;
        currentSpawnLoc = spawnLoc;
        currentType = SpawningType;

        GameObject currentGroup = Instantiate(roomGroupPrefab, spawnLoc,transform.rotation);
        currentParent = currentGroup.GetComponent<RoomGroupParent>();
        currentParent.name = currentType.name + " Room Group";
        currentParent.AmountOfRooms = RoomAmount;
        yield return new WaitForSeconds(0.3f * (FastLoading ? 0 : 1));
        if (SpawningType.HasStartRoom)
        {
            GameObject startRoomPref = SpawningType.RoomTypeStartRoom;
            GameObject startRoom = Instantiate(startRoomPref, spawnLoc, transform.rotation);
            startRoom.transform.parent = currentParent.transform;
            currentParent.StartingRoom = startRoom;
            currentParent.StartingCarriage = startRoom.GetComponent<CarriageClass>();
            //For spawning the first room
            currentParent._initializedRooms.Add(startRoom);
            currentParent._initializedCarriages.Add(startRoom.GetComponent<CarriageClass>());
            if (currentParent.StartingCarriage.PlayerSpawnPoint && player && tpPlayer)
            {
                player.transform.position = currentParent.StartingCarriage.PlayerSpawnPoint.transform.position;
                player.SetActive(true);
            }
        }

        yield return new WaitForSeconds(0.1f * (FastLoading ? 0 : 1));

        allTotalPossibleRooms = new List<RoomClass>(SpawningType.AllRoomsInType);

        for (int i = 0; i < RoomAmount; i++)
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

        GameObject endRoom = Instantiate(SpawningType.RoomTypeEndRoom);
        PositionGeneratedRoom(endRoom, currentParent._initializedRooms[currentParent._initializedRooms.Count - 1]);
        currentParent.EndingRoom = endRoom;
        endRoom.transform.parent = currentParent.transform;
        StartCoroutine(GenerateNavmesh());

        for (int i = 0; i < currentParent._initializedRooms.Count; i++)
        {
            if (i <= RoomApproachSize)
            {
                currentParent._initializedCarriages[i].OnApproach(true);
            }
            else
            {
                currentParent._initializedCarriages[i].OnRecede(true);
            }
        }

        _initializedRoomGroups.Add(currentParent);
        IsGenerating = false;
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

    void PositionGeneratedRoom(GameObject room, Vector3 position)
    {
        CarriageClass currentCarriage = room.GetComponent<CarriageClass>();

        Transform entry = currentCarriage.EntryPoint;


        Vector3 entryOffset = room.transform.position - entry.position;

        room.transform.position = position + entryOffset;
        room.transform.rotation = transform.rotation * Quaternion.Inverse(entry.rotation);

        currentCarriage.generationClass = this;

        currentCarriage.SpawnItems();
    }

#if UNITY_EDITOR

    public void RegenerateRooms()
    {
        if (IsGenerating) return;
        for(int i = 0;i < _initializedRoomGroups[0]._initializedRooms.Count;i++)
        {
            _initializedRoomGroups[0]._initializedCarriages[i].DespawnItems();
            Destroy(_initializedRoomGroups[0]._initializedRooms[i]);
        }
        Destroy(_initializedRoomGroups[0]);
        _initializedRoomGroups = new List<RoomGroupParent>();
        prevRoomCarriage = null;
        prevRoomClassName = null;
        StartCoroutine(GenerateRooms(Rooms,DefaultSpawnLocation.position,AmountOfRooms));
    }
#endif

    private CarriageClass prevRoomCarriage;
    private void GiveRoomEvents(CarriageClass room, RoomClass roomClass)
    {
        List<EventClassScriptable> allowedEvents = new List<EventClassScriptable>(roomClass.AllowedEvents);

        if (prevRoomCarriage != null && !Rooms.AllowEventDupes)
        {
            List<EventClassScriptable> prevRoomEvents = prevRoomCarriage._selectedEventClasses;
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

                EventClassScriptable _chosenEvent = allowedEvents[index];

                AddEventToRoom(room, _chosenEvent);

                allowedEvents.RemoveAt(index);

                foreach (EventClassScriptable removeEvent in _chosenEvent.removeEvents)
                {
                    if (allowedEvents.Contains(removeEvent))
                    {
                        allowedEvents.Remove(removeEvent);
                    }
                }
            }
        }

        prevRoomCarriage = room;

    }

    public static void AddEventToRoom(CarriageClass room, EventClassScriptable eventClass, bool enter = false)
    {
        System.Type eventType = EventRefs.Instance.GetClassFromScriptable(eventClass).GetType();
        EventClass even = (EventClass)room.gameObject.AddComponent(eventType);
        even.scriptable = eventClass;
        even.id = eventClass.id;
        even.Generate(room);

        if (enter)
        {
            even.FirstEnter(room);
        }
        bool isClose = false;
        if (PlrRefs.inst.PlayerController.CurrentCarriage)
        {
            int ind = PlrRefs.inst.PlayerController.CurrentCarriage.roomIndex;
            for (int i = mainInstance.RoomApproachSize; i < mainInstance.RoomApproachSize; i++)
            {
                if (room.roomIndex == PlrRefs.inst.PlayerController.CurrentCarriage.roomIndex + i)
                {
                    even.FirstApproach(room);
                    isClose = true;
                }
            }
        }
        if (!isClose)
        {
            even.Recede(room);
        }

        room.spawnedEventClasses.Add(even);
        room._selectedEventClasses.Add(eventClass);
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
            if(currentParent._initializedRooms.Count != 0)
            {
                previousRoom = currentParent._initializedRooms[index];
                PositionGeneratedRoom(randomRoom, previousRoom);
            }
            else
            {
                PositionGeneratedRoom(randomRoom, currentSpawnLoc);
            }

            randomRoom.name = "Room" + index + selectedroom.RoomName;

            CarriageClass randomCarriage = randomRoom.GetComponent<CarriageClass>();
            randomCarriage.previousCarriage = previousRoom?.GetComponent<CarriageClass>();
            randomCarriage.roomIndex = index + 1;
            randomCarriage.roomParent = currentParent;
            if (previousRoom) { previousRoom.GetComponent<CarriageClass>().nextCarriage = randomCarriage; }

            currentParent._initializedRooms.Add(randomRoom);
            currentParent._initializedCarriages.Add(randomCarriage);
            randomRoom.transform.parent = currentParent.transform;
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
    public void EnterRoom(int index,RoomGroupParent parent)
    {
        if(currentRoomIndex == index) { return; }
        int acceptableI = -RoomApproachSize;
        if (index >= RoomApproachSize + 1)
        {
            parent._initializedCarriages[index - RoomApproachSize - 1].OnRecede();
            if(index - currentRoomIndex > 1 && index >= RoomApproachSize + 2)
            {
                parent._initializedCarriages[index - RoomApproachSize - 2].OnRecede();
            }
        }
        else
        {
            acceptableI = 1;
        }
        for (int i = acceptableI; i < RoomApproachSize + 1; i++)
        {
            if(i + index > parent.AmountOfRooms) { continue; }
            parent._initializedCarriages[index + i]?.OnApproach();
        }

        currentRoomIndex = index;
        if(index + RoomApproachSize + 1 < parent.AmountOfRooms)
        {
            parent._initializedCarriages[index + RoomApproachSize + 1]?.OnRecede();
        }
    }
}
