using Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class CarriageClass : MonoBehaviour
{
    [Header("RoomInfo")]
    public Transform EntryPoint;
    public Transform ExitPoint;

    public int roomIndex = -999;

    private bool _enterTriggered;
    private bool _exitTriggered;
    private bool hasBeenLoaded = false;
    private bool isLoaded = true;

    [SerializeField] private UnityEvent OnRecedeEvent;
    [SerializeField] private UnityEvent OnApproachEvent;
    [SerializeField] private UnityEvent OnFirstApproachEvent;

    [Header("Player")]
    public Transform PlayerSpawnPoint;
    private bool playerInside = false;
    private PlayerController _player;

    [Header("Event")]
    public Transform[] SpawnPoints;
    public Transform NodeHolder;
    public Transform InstanceHolder;

    public List<EventClass> spawnedEventClasses;
    public List<EventClassScriptable> _selectedEventClasses;

    [Header("Items")]
    [SerializeField] private List<InventoryItem> _allowedDrops;
    [SerializeField] private int _maxAmountOfItems = 1;
    public List<Transform> AllItemSpawnLocations;
    public List<Transform> RemainingSpawnLocations;
    private List<GameObject> spawnedItems = new List<GameObject>();

    [Header("References")]
    public Generation generationClass;
    public RoomEventRefs RoomEventRefs;
    public RoomGroupParent roomParent;
    public RoomSetupManager RoomSetup;

    public CarriageClass previousCarriage;
    public CarriageClass nextCarriage;

    //Item Spawning Section
    public void SpawnRoomItems()
    {
        if (_maxAmountOfItems == 0) return;

        if (RemainingSpawnLocations.Count > 0)
        {
            for (int i = 0; i < Random.Range(0, _maxAmountOfItems + 1); i++)
            {
                InventoryItem chosenItem = _allowedDrops[Random.Range(0, _allowedDrops.Count)];
                Transform randSpot = GetRandomItemSpot();
                SpawnItem(chosenItem, randSpot);
            }
        }
        else if (AllItemSpawnLocations.Count > 0)
        {
            Debug.LogWarning("No unspawned item spots, forced to double up some spots in room: " + gameObject.name);
            for (int i = 0; i < Random.Range(0, _maxAmountOfItems + 1); i++)
            {
                InventoryItem chosenItem = _allowedDrops[Random.Range(0, _allowedDrops.Count)];
                Transform randSpot = GetRandomItemSpot();
                SpawnItem(chosenItem, randSpot);
            }
        }
        else
        {
            Debug.LogError("No item spawn points found for room: " + gameObject.name);
        }
    }
    public void GetItemSpawnPoints()
    {
        if (_maxAmountOfItems == 0) return;

        AllItemSpawnLocations = new List<Transform>();
        RemainingSpawnLocations = new List<Transform>();

        //This prevents spawning in parent empties, since all item spots have a mesh filter for easy prefab changing
        List<MeshFilter> _spawnPoints = SpawnPoints[0].GetComponentsInChildren<MeshFilter>(false).ToList();
        foreach (MeshFilter spawnPoint in _spawnPoints)
        {
            AllItemSpawnLocations.Add(spawnPoint.transform);
            RemainingSpawnLocations.Add(spawnPoint.transform);
        }
    }
    public Transform GetRandomItemSpot()
    {
        Transform randomLocation = transform;
        if (RemainingSpawnLocations.Count > 0)
        {
            randomLocation = RemainingSpawnLocations[Random.Range(0, RemainingSpawnLocations.Count)];
            RemainingSpawnLocations.Remove(randomLocation);
        }
        else if (AllItemSpawnLocations.Count > 0)
        {
            randomLocation = AllItemSpawnLocations[Random.Range(0, AllItemSpawnLocations.Count)];
            Debug.LogWarning("No unspawned item spots, forced to double up a spot " + gameObject.name);
        }
        else
        {
            Debug.LogError("Both spots have no places " + gameObject.name);
        }
        return randomLocation;
    }

    public GameObject SpawnItem(InventoryItem item,Transform location)
    {
        GameObject newDroppedItem = Instantiate(item.HoldObject, location.position, location.rotation * item.HoldObject.transform.rotation);
        spawnedItems.Add(newDroppedItem);
        return newDroppedItem;
    }
    public GameObject SpawnItem(GameObject item, Transform location)
    {
        GameObject newDroppedItem = Instantiate(item, location.position, location.rotation * item.transform.rotation);
        spawnedItems.Add(newDroppedItem);
        return newDroppedItem;
    }
#if UNITY_EDITOR
    //All functions under the "#if Unity_Editor" are for the editor only and so are made with quantity over quality, expect lower quality code.
    public void DespawnItems()
    {
        for(int i = 0; i <  spawnedItems.Count; i++)
        {
            if (spawnedItems[i] == null) { continue; }
            //Dont despawn the keys please
            if (spawnedItems[i].GetComponent<ItemImportance>()) { continue; }
            Destroy(spawnedItems[i]);
        }
    }

    public void SpawnItemsAllSlots()
    {
        StartCoroutine(SpawnItemAfterDelay());
    }

    //Needed due to despawning not being fast enough with removing hitboxes, causing new items to get launched.
    private IEnumerator SpawnItemAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        if (SpawnPoints.Length > 0) {
            List<Transform> _spawnPoints = SpawnPoints[0].GetComponentsInChildren<Transform>().ToList();
            _spawnPoints.RemoveAt(0);
            if (_spawnPoints.Count > 0)
            {
                for (int i = 0; i < _spawnPoints.Count; i++)
                {
                    Transform setLocation = _spawnPoints[i];
                    InventoryItem chosenItem = _allowedDrops[Random.Range(0, _allowedDrops.Count)];
                    SpawnItem(chosenItem, setLocation);
                }
            }
        }
    }
#endif

    //---End of item section
    void Start()
    {
        _player = PlrRefs.inst.PlayerController;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        //3 = Player Layer
        if (other.gameObject.layer == 3 && !playerInside)
        {
            _player.CurrentRoom = gameObject;
            _player.CurrentCarriage = this;
            if(roomIndex != -999)
            {
                generationClass.EnterRoom(roomIndex,roomParent);
            }
            playerInside = true;

            if (!_enterTriggered)
            {
                _enterTriggered = true;
                foreach (EventClass selectedEventClass in spawnedEventClasses)
                    selectedEventClass.FirstEnter(this);
            }
            else
            {
                foreach (EventClass selectedEventClass in spawnedEventClasses)
                    selectedEventClass.RepeatEnter(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //3 = Player Layer
        if (other.gameObject.layer == 3 && playerInside)
        {
            playerInside = false;

            // Check if player left through the back (lower Z position than the carriage)
            if (!_exitTriggered && PlrRefs.inst.transform.position.z > transform.position.z)
            {
                _exitTriggered = true;

                foreach (EventClass selectedEventClass in spawnedEventClasses)
                    selectedEventClass.FirstExit(this);
            }
            else
            {
                if (!_exitTriggered)
                {
                    foreach (EventClass selectedEventClass in spawnedEventClasses)
                        selectedEventClass.EarlyExit(this);
                }
                else
                {
                    foreach (EventClass selectedEventClass in spawnedEventClasses)
                        selectedEventClass.RepeatExit(this);
                }
            }
        }
    }

    /// <summary>
    /// Visually load room that is a few rooms away from player, lower quality in things like ai
    /// </summary>
    /// <param name="overrideState"></param>
    public void OnApproach(bool overrideState = false)
    {
        if (isLoaded && !overrideState) { return; }
        isLoaded = true;
        if (!hasBeenLoaded)
        {
            //first time loaded stuff
            OnFirstApproachEvent.Invoke();
            foreach (EventClass @event in spawnedEventClasses)
            {
                @event.FirstApproach(this);
            }
            hasBeenLoaded = true;
        }
        OnApproachEvent.Invoke();
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            if(spawnedItems[i] != null)
            {
                spawnedItems[i].GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        foreach (EventClass @event in spawnedEventClasses)
        {
            @event.RepeatApproach(this);
        }
    }

    /// <summary>
    /// Unload room thats far away from player
    /// </summary>
    /// <param name="overrideState"></param>
    public void OnRecede(bool overrideState = false)
    {
        if (!isLoaded && !overrideState) { return; }
        isLoaded = false;
        OnRecedeEvent.Invoke();
        for(int i = 0; i < spawnedItems.Count; i++)
        {
            if (spawnedItems[i] != null)
            {
                spawnedItems[i].GetComponent<Rigidbody>().isKinematic = true;
            }
        }
        foreach (EventClass @event in spawnedEventClasses)
        {
            @event.Recede(this);
        }
    }

    public EventClass CheckAndGetEvent(int id)
    {
        for(int i = 0;i < spawnedEventClasses.Count;i++)
        {
            if((spawnedEventClasses[i].id == id))
            {
                return spawnedEventClasses[i];
            }
        }
        return null;
    }
}
