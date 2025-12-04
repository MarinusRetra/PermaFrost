using System.Collections.Generic;
using UnityEngine;

public class CarriageClass : MonoBehaviour
{
    [Header("Transforms")]
    public Transform EntryPoint;
    public Transform ExitPoint;
    public Transform MainObject;
    public Transform SpawnPoint;

    [SerializeField] private List<Transform> _spawnPoints;
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


    public void SpawnItems()
    {
        if (_spawnPoints.Count > 0)
        {
            for(int i = 0; i < Random.Range(0,_maxAmountOfItems + 1); i++)
            {
                Transform randomLocation = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
                InventoryItem chosenItem = _allowedDrops[Random.Range(0, _allowedDrops.Count)];
                GameObject newDroppedItem = Instantiate(chosenItem.HoldObject, randomLocation.position, Quaternion.identity);

                //prevent 2 items in 1 spot
                _spawnPoints.Remove(randomLocation);
            }
        }
    }

    void Start()
    {
        if (_allowedEvents.Count > 0 && _amountOfEvents > 0)
        {
            int count = Mathf.Min(_amountOfEvents, _allowedEvents.Count);
            List<EventClass> availableEvents = new List<EventClass>(_allowedEvents);

            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, availableEvents.Count);
                EventClass _chosenEvent = availableEvents[randomIndex];
                _selectedEventClasses.Add(_chosenEvent);
                availableEvents.RemoveAt(randomIndex);
                _chosenEvent.Generated(gameObject);
            }
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && !playerInside && _selectedEventClasses.Count > 0 && !_enterTriggered)
        {
            // Check if player is in front (higher Z position than the carriage)
            if (generationClass.player.transform.position.z < transform.position.z)
            {
                playerInside = true;
                _enterTriggered = true;

                foreach (EventClass selectedEventClass in _selectedEventClasses)
                    selectedEventClass.Entered(gameObject);
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
                    selectedEventClass.Exited(gameObject);
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
                selectedEventClass.Triggered(gameObject);
        }
    }
}
