using Gameplay;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Generation : MonoBehaviour
{
    public static Generation mainInstance;

    [Header("Required To Generate")]
    public RoomTypeScriptable Rooms;
    public RoomTypeScriptable BackupRooms;
    [SerializeField] private GameObject roomGroupPrefab;
    [SerializeField] private NavMeshSurface _meshSurface;
    public Transform DefaultSpawnLocation;

    [Header("Generating")]
    public bool FastLoading = false;
    public bool IsGenerating = false;
    public int AmountOfRooms = 15;

    //Currently spawning stuffs
    private RoomGroupParent currentGroup;
    private Vector3 currentSpawnLoc;
    private RoomTypeScriptable currentType;
    private int currentHeightValue = 0;
    private string prevRoomClassName;
    private CarriageClass prevRoomCarriage;
    bool generatingRoom = false;
    List<RoomClass> allPossibleRooms = new List<RoomClass>();

    [Header("Misc")]
    public List<RoomGroupParent> _initializedRoomGroups = new List<RoomGroupParent>();
    public GameObject player;
    private int currentPlayerRoomIndex = -999;
    public int RoomApproachSize = 2;

    void Start()
    {
        mainInstance = this;    
        StartCoroutine(GenerateRooms(Rooms,DefaultSpawnLocation.position,AmountOfRooms));
    }

    public IEnumerator GenerateRooms(RoomTypeScriptable SpawningType, Vector3 spawnLoc, int RoomAmount, bool tpPlayer = true)
    {
        //Reset Everything
        IsGenerating = true;
        currentHeightValue = 0;
        currentSpawnLoc = spawnLoc;
        currentType = SpawningType;

        //Spawn Roomgroup
        GameObject newGroup = Instantiate(roomGroupPrefab, spawnLoc, transform.rotation);
        currentGroup = newGroup.GetComponent<RoomGroupParent>();
        currentGroup.name = currentType.name + " Room Group";
        currentGroup.AmountOfRooms = RoomAmount;

        yield return new WaitForSeconds(0.3f * (FastLoading ? 0 : 1));
        if (SpawningType.HasStartRoom)
        {
            GameObject startRoomPref = SpawningType.RoomTypeStartRoom;
            GameObject startRoom = Instantiate(startRoomPref, spawnLoc, transform.rotation);
            startRoom.transform.parent = currentGroup.transform;

            currentGroup.StartingRoom = startRoom;
            currentGroup.StartingCarriage = startRoom.GetComponent<CarriageClass>();
            currentGroup._initializedRooms.Add(startRoom);
            currentGroup._initializedCarriages.Add(startRoom.GetComponent<CarriageClass>());

            if (currentGroup.StartingCarriage.PlayerSpawnPoint && player && tpPlayer)
            {
                player.transform.position = currentGroup.StartingCarriage.PlayerSpawnPoint.transform.position;
                player.SetActive(true);
            }
        }

        yield return new WaitForSeconds(0.1f * (FastLoading ? 0 : 1));

        allPossibleRooms = new List<RoomClass>(SpawningType.AllRoomsInType);

        for (int i = 0; i < RoomAmount; i++)
        {
            SelectWeightedRoom(i);
            yield return new WaitForSeconds(0.3f * (FastLoading ? 0.1f : 1));

            //wait longer while room is still being generated
            for (int j = 0; j < 10; j++)
            {
                if (generatingRoom)
                {
                    yield return new WaitForSeconds(0.3f * (FastLoading ? 0.1f : 1));
                }
            }
        }

        GameObject endRoom = Instantiate(SpawningType.RoomTypeEndRoom);
        PositionGeneratedRoom(endRoom, currentGroup._initializedRooms[currentGroup._initializedRooms.Count - 1]);
        currentGroup.EndingRoom = endRoom;
        endRoom.transform.parent = currentGroup.transform;
        StartCoroutine(GenerateNavmesh());

        for (int i = 0; i < currentGroup._initializedRooms.Count; i++)
        {
            if (i <= RoomApproachSize)
            {
                currentGroup._initializedCarriages[i].OnApproach(true);
            }
            else
            {
                currentGroup._initializedCarriages[i].OnRecede(true);
            }
        }

        _initializedRoomGroups.Add(currentGroup);
        IsGenerating = false;
    }

    //Room Section
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
    private void SelectWeightedRoom(int index)
    {
        generatingRoom = true;
        List<RoomClass> allCurrentPossibleRooms = new List<RoomClass>(allPossibleRooms);

        //Remove previous room from the pool (if that setting is turned on)
        if (index > 0 && !Rooms.AllowDupes)
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
            //Try 5 times to get a random room
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
            Debug.LogError("Room wasnt able to spawn.");
        }
        else
        {
            SpawnWeightedRoom(index, selectedroom);
        }
        generatingRoom = false;
    }

    private CarriageClass SpawnWeightedRoom(int index, RoomClass selectedroom)
    {
        GameObject randomRoom = Instantiate(selectedroom.Room);
        CarriageClass randomCarriage = randomRoom.GetComponent<CarriageClass>();
        GameObject previousRoom = null;
        if (currentGroup._initializedRooms.Count != 0)
        {
            if (currentGroup._initializedRooms.Count == index)
            {
                previousRoom = currentGroup._initializedRooms[index - 1];
            }
            else
            {
                previousRoom = currentGroup._initializedRooms[index];
            }
            PositionGeneratedRoom(randomRoom, previousRoom);
        }
        else
        {
            PositionGeneratedRoom(randomRoom, currentSpawnLoc);
        }

        randomRoom.name = "Room" + index + selectedroom.RoomName;

        randomCarriage.previousCarriage = previousRoom?.GetComponent<CarriageClass>();
        randomCarriage.roomIndex = index + 1;
        randomCarriage.roomParent = currentGroup;

        if (randomCarriage.RoomSetup)
        {
            randomCarriage.RoomSetup.ApplyVariant();
        }

        randomCarriage.GetItemSpawnPoints();
        randomCarriage.SpawnRoomItems();

        GiveRoomEvents(randomCarriage, selectedroom);
        if (previousRoom) { previousRoom.GetComponent<CarriageClass>().nextCarriage = randomCarriage; }

        currentGroup._initializedRooms.Add(randomRoom);
        currentGroup._initializedCarriages.Add(randomCarriage);
        prevRoomClassName = selectedroom.RoomName;

        randomRoom.transform.parent = currentGroup.transform;
        _meshSurface.UpdateNavMesh(_meshSurface.navMeshData);

        if (selectedroom.onlySpawnOnce == true)
        {
            allPossibleRooms.Remove(selectedroom);
        }
        return randomCarriage;
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
    }

    void PositionGeneratedRoom(GameObject room, Vector3 position)
    {
        CarriageClass currentCarriage = room.GetComponent<CarriageClass>();

        Transform entry = currentCarriage.EntryPoint;


        Vector3 entryOffset = room.transform.position - entry.position;

        room.transform.position = position + entryOffset;
        room.transform.rotation = transform.rotation * Quaternion.Inverse(entry.rotation);

        currentCarriage.generationClass = this;
    }

    private void GiveRoomEvents(CarriageClass room, RoomClass roomClass)
    {
        List<EventClassScriptable> allowedEvents = new List<EventClassScriptable>(roomClass.AllowedEvents);

        //Remove previous rooms events if that setting is turned on
        if (prevRoomCarriage != null && !Rooms.AllowEventDupes)
        {
            List<EventClassScriptable> prevRoomEvents = prevRoomCarriage._selectedEventClasses;
            for (int i = 0; i < prevRoomEvents.Count; i++)
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

            //Hard-coded event scaling, there is not enough time for us to make a good system for this.
            if (room.roomIndex < 3) { count = Mathf.Min(1, count); }
            if (room.roomIndex >= 3 && room.roomIndex < 11) { count = Mathf.Min(2, count); }

            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, allowedEvents.Count);
                int index = Mathf.Min(randomIndex, allowedEvents.Count);

                if (allowedEvents.Count == 0) { prevRoomCarriage = room; return; }

                EventClassScriptable chosenEvent = allowedEvents[index];

                AddEventToRoom(room, chosenEvent);

                allowedEvents.RemoveAt(index);

                foreach (EventClassScriptable removeEvent in chosenEvent.removeEvents)
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

        room.spawnedEventClasses.Add(even);
        room._selectedEventClasses.Add(eventClass);
    }

    //---End of rooms section

#if UNITY_EDITOR
    //All functions under the "#if Unity_Editor" are for the editor only and so are made with quantity over quality, expect lower quality code.
    public void RegenerateRooms()
    {
        if (IsGenerating) return;
        for (int i = 0; i < _initializedRoomGroups[0]._initializedRooms.Count; i++)
        {
            _initializedRoomGroups[0]._initializedCarriages[i].DespawnItems();
            Destroy(_initializedRoomGroups[0]._initializedRooms[i]);
        }
        Destroy(_initializedRoomGroups[0].EndingRoom);
        Destroy(_initializedRoomGroups[0].gameObject);
        _initializedRoomGroups = new List<RoomGroupParent>();
        prevRoomCarriage = null;
        prevRoomClassName = null;
        StartCoroutine(GenerateRooms(Rooms, DefaultSpawnLocation.position, AmountOfRooms));
    }

    public void SETUPManualPlacing(GameObject startingRoom)
    {
        //destroy previous generation
        if (IsGenerating) { return; }
        for (int i = 0; i < _initializedRoomGroups[0]._initializedRooms.Count; i++)
        {
            _initializedRoomGroups[0]._initializedCarriages[i].DespawnItems();
            Destroy(_initializedRoomGroups[0]._initializedRooms[i]);
        }
        Destroy(_initializedRoomGroups[0].EndingRoom);
        Destroy(_initializedRoomGroups[0].gameObject);
        _initializedRoomGroups = new List<RoomGroupParent>();
        prevRoomCarriage = null;
        prevRoomClassName = null;

        //start new generating
        IsGenerating = true;
        currentHeightValue = 0;
        currentSpawnLoc = DefaultSpawnLocation.position;

        //spawn roomgroup
        GameObject currentGroup = Instantiate(roomGroupPrefab, currentSpawnLoc, transform.rotation);
        this.currentGroup = currentGroup.GetComponent<RoomGroupParent>();
        this.currentGroup.name = "CUSTOM Room Group";

        //spawn start room
        GameObject startRoom = Instantiate(startingRoom, currentSpawnLoc, transform.rotation);
        startRoom.transform.parent = this.currentGroup.transform;
        this.currentGroup.StartingRoom = startRoom;
        this.currentGroup.StartingCarriage = startRoom.GetComponent<CarriageClass>();
        this.currentGroup._initializedRooms.Add(startRoom);
        this.currentGroup._initializedCarriages.Add(startRoom.GetComponent<CarriageClass>());
        if (this.currentGroup.StartingCarriage.PlayerSpawnPoint && player)
        {
            player.transform.position = this.currentGroup.StartingCarriage.PlayerSpawnPoint.transform.position;
            player.SetActive(true);
        }
        _initializedRoomGroups.Add(this.currentGroup);
    }
    public void RemoveRoom()
    {
        if(currentGroup._initializedCarriages.Count - 1 == 0) { return; }
        CarriageClass lascer = currentGroup._initializedCarriages[currentGroup._initializedCarriages.Count - 1];
        currentGroup._initializedCarriages.Remove(lascer);
        currentGroup._initializedRooms.Remove(lascer.gameObject);
        lascer.DespawnItems();
        Destroy(lascer.gameObject);
    }
    public void ClearRooms()
    {
        //destroy previous generation
        for (int i = _initializedRoomGroups[0]._initializedRooms.Count - 1; i > 0; i--)
        {
            _initializedRoomGroups[0]._initializedCarriages[i].DespawnItems();
            currentGroup._initializedCarriages.Remove(currentGroup._initializedCarriages[i]);
            GameObject room = currentGroup._initializedRooms[i];
            currentGroup._initializedRooms.Remove(currentGroup._initializedRooms[i]);
            Destroy(room);
        }
        prevRoomCarriage = null;
        prevRoomClassName = null;

        //start new generating
        currentHeightValue = 0;
        currentSpawnLoc = DefaultSpawnLocation.position;
        prevRoomCarriage = _initializedRoomGroups[0].StartingCarriage;
    }
    public void AddRoom(RoomClass room)
    {
        CarriageClass newThing = SpawnWeightedRoom(currentGroup._initializedRooms.Count, room);
        newThing.OnApproach();
    }
#endif

    private IEnumerator GenerateNavmesh()
    {
        yield return new WaitForEndOfFrame();
        _meshSurface.UpdateNavMesh(_meshSurface.navMeshData);
    }

    public void EnterRoom(int index,RoomGroupParent parent)
    {
        if(currentPlayerRoomIndex == index) { return; }
        int acceptableI = -RoomApproachSize;
        if (index >= RoomApproachSize + 1)
        {
            parent._initializedCarriages[index - RoomApproachSize - 1].OnRecede();
            if(index - currentPlayerRoomIndex > 1 && index >= RoomApproachSize + 2)
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

        currentPlayerRoomIndex = index;
        if(index + RoomApproachSize + 1 < parent.AmountOfRooms)
        {
            parent._initializedCarriages[index + RoomApproachSize + 1]?.OnRecede();
        }
    }
}
