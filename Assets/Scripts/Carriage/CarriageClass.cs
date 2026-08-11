using Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class CarriageClass : MonoBehaviour
{
    [Header("Transforms")]
    public Transform EntryPoint;
    public Transform ExitPoint;
    public Transform PlayerSpawnPoint;

    public Transform[] SpawnPoints;
    public Transform Holder;
    [SerializeField] private List<InventoryItem> _allowedDrops;
    public Generation generationClass;
    public int roomIndex = -999;

    [SerializeField] private bool _enterTriggered;
    [SerializeField] private bool _exitTriggered;
    [SerializeField] private bool _triggerTriggered;

    [SerializeField] private UnityEvent OnRecedeEvent;
    [SerializeField] private UnityEvent OnApproachEvent;
    [SerializeField] private UnityEvent OnFirstApproachEvent;

    private bool playerInside = false;
    public List<EventClass> spawnedEventClasses;
    public List<EventClassScriptable> _selectedEventClasses;
    [SerializeField] private int _maxAmountOfItems = 1;

    public CarriageClass previousCarriage;
    public CarriageClass nextCarriage;

    public Transform NodeHolder;

    private PlayerController _player;

    private List<GameObject> spawnedItems = new List<GameObject>();

    private bool hasBeenLoaded = false;
    private bool isLoaded = true;

    public RoomEventRefs RoomEventRefs;

    public RoomGroupParent roomParent;

    public List<Transform> AllItemSpawnLocations;
    public List<Transform> SanitizedSpawnLocations;

    public RoomSetupManager RoomSetup;


    public void GetItemSpawnPoints()
    {
        if (_maxAmountOfItems == 0) return;
        AllItemSpawnLocations = new List<Transform>();
        SanitizedSpawnLocations = new List<Transform>();
        List<MeshFilter> _spawnPoints = SpawnPoints[0].GetComponentsInChildren<MeshFilter>(false).ToList();
        foreach (MeshFilter spawnPoint in _spawnPoints)
        {
            AllItemSpawnLocations.Add(spawnPoint.transform);
            SanitizedSpawnLocations.Add(spawnPoint.transform);
        }
    }
    public void SpawnRoomItems()
    {
        if (_maxAmountOfItems == 0) return;
        if (SanitizedSpawnLocations.Count > 0)
        {
            for(int i = 0; i < Random.Range(0, _maxAmountOfItems + 1); i++)
            {
                InventoryItem chosenItem = _allowedDrops[Random.Range(0, _allowedDrops.Count)];
                Transform randSpot = GetRandomItemSpot();
                SpawnItem(chosenItem, randSpot);
            }
        }
        else if(AllItemSpawnLocations.Count > 0)
        {
            Debug.LogWarning("No unspawned item spots, forced to double up ALL spots " + gameObject.name);
            for (int i = 0; i < Random.Range(0, _maxAmountOfItems + 1); i++)
            {
                InventoryItem chosenItem = _allowedDrops[Random.Range(0, _allowedDrops.Count)];
                Transform randSpot = GetRandomItemSpot();
                SpawnItem(chosenItem, randSpot);
            }
        }
        else
        {
            Debug.LogError("No item spawn points found. " + gameObject.name);
        }
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

    public Transform GetRandomItemSpot()
    {
        Transform randomLocation = transform;
        if (SanitizedSpawnLocations.Count > 0)
        {
            randomLocation = SanitizedSpawnLocations[Random.Range(0, SanitizedSpawnLocations.Count)];
            SanitizedSpawnLocations.Remove(randomLocation);
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
#if UNITY_EDITOR
    //these functions are only used for the player editor tab, do not worry about them too much
    public void DespawnItems()
    {
        for(int i = 0; i <  spawnedItems.Count; i++)
        {
            if (spawnedItems[i] == null) { continue; }
            if(spawnedItems[i].GetComponent<ItemImportance>()) { continue; }
            Destroy(spawnedItems[i]);
        }
    }

    public void SpawnItemsAllSlots()
    {
        StartCoroutine(LilbroNeedsToWaitCauseHitboxes());
    }

    private IEnumerator LilbroNeedsToWaitCauseHitboxes()
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

    void Start()
    {
        _player = PlrRefs.inst.PlayerController;
    }

    
    private void OnTriggerEnter(Collider other)
    {
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
