using Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarriageClass : MonoBehaviour
{
    [Header("Transforms")]
    public Transform EntryPoint;
    public Transform ExitPoint;
    public Transform PlayerSpawnPoint;

    public Transform[] SpawnPoints;
    public Transform Holder;
    [SerializeField] private List<InventoryItem> _allowedDrops;
    [SerializeField] private List<EventClass> _allowedEvents;
    public Generation generationClass;

    [SerializeField] private bool _enterTriggered;
    [SerializeField] private bool _exitTriggered;
    [SerializeField] private bool _triggerTriggered;

    private bool playerInside = false;
    public List<EventClass> _selectedEventClasses;
    [SerializeField] private int _amountOfEvents;
    [SerializeField] private int _maxAmountOfItems = 1;

    public CarriageClass previousCarriage;

    private PlayerController _player;

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
                //newDroppedItem.transform.parent = transform;

                //prevent 2 items in 1 spot
                _spawnPoints.Remove(randomLocation);
            }
        }
    }

    void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        if (_allowedEvents.Count > 0 && _amountOfEvents > 0)
        {
            int count = Mathf.Min(_amountOfEvents, _allowedEvents.Count);
            List<EventClass> availableEvents = new List<EventClass>(_allowedEvents);

            if (previousCarriage)
            {
                for (int i = 0; i < previousCarriage._selectedEventClasses.Count; i++)
                {
                    if (availableEvents.Contains(previousCarriage._selectedEventClasses[i]))
                    {
                        availableEvents.Remove(previousCarriage._selectedEventClasses[i]);
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                if(availableEvents.Count == 0)
                {
                    break;
                }
                int randomIndex = Random.Range(0, availableEvents.Count);
                EventClass _chosenEvent = availableEvents[randomIndex];
                _selectedEventClasses.Add(_chosenEvent);
                availableEvents.RemoveAt(randomIndex);
                _chosenEvent.Generated(this);
                if(_chosenEvent is WindowEvent || _chosenEvent is HotDudeEvent)
                {
                    for(int j = 0; j < availableEvents.Count; j++)
                    {
                        if (availableEvents[j] is HotDudeEvent || availableEvents[j] is WindowEvent)
                        {
                            availableEvents.RemoveAt(j);
                        }
                    }
                }
            }
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && !playerInside)
        {
            _player.CurrentRoom = gameObject;

            if(_selectedEventClasses.Count > 0 && !_enterTriggered)
            {
                // Check if player is in front (higher Z position than the carriage)
                if (generationClass.player.transform.position.z < transform.position.z)
                {
                    playerInside = true;
                    _enterTriggered = true;

                    foreach (EventClass selectedEventClass in _selectedEventClasses)
                        selectedEventClass.Entered(this);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3 && playerInside && _selectedEventClasses.Count > 0 && !_exitTriggered)
        {
            // Check if player left through the back (lower Z position than the carriage)
            if (generationClass.player.transform.position.z > transform.position.z)
            {
                playerInside = false;
                _exitTriggered = true;

                foreach (EventClass selectedEventClass in _selectedEventClasses)
                    selectedEventClass.Exited(this);
            }
        }
    }
    
    private void OnTriggerCalled()
    {
        if (_triggerTriggered != true)
        {
            _triggerTriggered = true;

            // Add your logic here
            foreach (EventClass selectedEventClass in _selectedEventClasses)
                selectedEventClass.Triggered(this);
        }
    }
}
