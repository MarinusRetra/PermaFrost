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
    public int roomIndex;

    [SerializeField] private bool _enterTriggered;
    [SerializeField] private bool _exitTriggered;
    [SerializeField] private bool _triggerTriggered;

    [SerializeField] private UnityEvent OnRecedeEvent;
    [SerializeField] private UnityEvent OnApproachEvent;
    [SerializeField] private UnityEvent OnFirstApproachEvent;

    private bool playerInside = false;
    public List<EventClass> _selectedEventClasses;
    [SerializeField] private int _maxAmountOfItems = 1;

    public CarriageClass previousCarriage;
    public CarriageClass nextCarriage;

    public Transform NodeHolder;

    private PlayerController _player;

    private List<GameObject> spawnedItems = new List<GameObject>();

    private bool hasBeenLoaded = false;
    private bool isLoaded = true;

    public void SpawnItems()
    {
        if (_maxAmountOfItems == 0) return;
        List<Transform> _spawnPoints = SpawnPoints[0].GetComponentsInChildren<Transform>().ToList();
        _spawnPoints.RemoveAt(0);
        if (_spawnPoints.Count > 0)
        {
            for(int i = 0; i < Random.Range(0,_maxAmountOfItems + 1); i++)
            {
                Transform randomLocation = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
                InventoryItem chosenItem = _allowedDrops[Random.Range(0, _allowedDrops.Count)];
                GameObject newDroppedItem = Instantiate(chosenItem.HoldObject, randomLocation.position, Quaternion.identity);

                //prevent 2 items in 1 spot
                _spawnPoints.Remove(randomLocation);
                spawnedItems.Add(newDroppedItem);
            }
        }
    }
#if UNITY_EDITOR
    //these functions are only used for the player editor tab, do not worry about them too much
    public void DespawnItems()
    {
        for(int i = 0; i <  spawnedItems.Count; i++)
        {
            if (spawnedItems[i] == null) { continue; }
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
        List<Transform> _spawnPoints = SpawnPoints[0].GetComponentsInChildren<Transform>().ToList();
        _spawnPoints.RemoveAt(0);
        if (_spawnPoints.Count > 0)
        {
            for (int i = 0; i < _spawnPoints.Count; i++)
            {
                Transform setLocation = _spawnPoints[i];
                InventoryItem chosenItem = _allowedDrops[Random.Range(0, _allowedDrops.Count)];
                GameObject newDroppedItem = Instantiate(chosenItem.HoldObject, setLocation.position, Quaternion.identity);
                spawnedItems.Add(newDroppedItem);
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
            generationClass.EnterRoom(roomIndex);
            playerInside = true;

            if (!_enterTriggered)
            {
                _enterTriggered = true;
                foreach (EventClass selectedEventClass in _selectedEventClasses)
                    selectedEventClass.FirstEnter(this);
            }
            else
            {
                foreach (EventClass selectedEventClass in _selectedEventClasses)
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
            if (!_exitTriggered && generationClass.player.transform.position.z > transform.position.z)
            {
                _exitTriggered = true;

                foreach (EventClass selectedEventClass in _selectedEventClasses)
                    selectedEventClass.FirstExit(this);
            }
            else
            {
                if (!_exitTriggered)
                {
                    foreach (EventClass selectedEventClass in _selectedEventClasses)
                        selectedEventClass.EarlyExit(this);
                }
                else
                {
                    foreach (EventClass selectedEventClass in _selectedEventClasses)
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
            foreach (EventClass @event in _selectedEventClasses)
            {
                @event.FirstApproach(this);
            }
            hasBeenLoaded = true;
        }
        OnApproachEvent.Invoke();
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            spawnedItems[i].GetComponent<Rigidbody>().isKinematic = false;
        }
        foreach(EventClass @event in _selectedEventClasses)
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
            spawnedItems[i].GetComponent<Rigidbody>().isKinematic = true;
        }
        foreach (EventClass @event in _selectedEventClasses)
        {
            @event.Recede(this);
        }
    }
}
