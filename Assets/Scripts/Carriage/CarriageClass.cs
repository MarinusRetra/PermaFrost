using System.Collections.Generic;
using UnityEngine;
using Gameplay;

public class CarriageClass : MonoBehaviour
{
    [Header("Transforms")]
    public Transform EntryPoint;
    public Transform ExitPoint;
    public Transform MainObject;
    public Transform SpawnPoint;

    [SerializeField] private List<Transform> _spawnPoints;
    // [SerializeField] private List<Inventory> _allowedDrops;
    [SerializeField] private List<EventClass> _allowedEvents;
    public Generation generationClass;

    [SerializeField] private bool _enterTriggered;
    [SerializeField] private bool _exitTriggered;
    [SerializeField] private bool _triggerTriggered;

    private bool playerInside = false;

    [SerializeField] private GameObject _droppedItemPrefab;
    [SerializeField] private List<EventClass> _selectedEventClasses;
    [SerializeField] private int _amountOfEvents;
    

    // public void SpawnRandomItem()
    // {
    //     //THIS FULLY BREAKS THE BUILD FIX BEFORE REACTIVATING
    //     return;
    //     if (_spawnPoints.Count > 0)
    //     {
    //         Transform randomLocation = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
    //         GameObject newDroppedItem = Instantiate(_droppedItemPrefab, randomLocation.position, Quaternion.identity);

    //         var interactObject = newDroppedItem.GetComponent<InteractObject>();
    //         if (interactObject != null)
    //         {
    //             InventoryItem inventoryItem = _allowedDrops[Random.Range(0, _allowedDrops.Count)];
    //             newDroppedItem.GetComponent<MeshRenderer>().material.color = inventoryItem.color;
    //             interactObject.InteractEvent.AddListener(() => OnItemInteracted(newDroppedItem, inventoryItem));
    //         }
    //     }
    // }

    // private void OnItemInteracted(GameObject item, InventoryItem inventoryItem)
    // {
    //     Debug.Log("Player interacted with " + item.name);

    //     PlayerInventory playerInventory = generationClass.player.GetComponent<PlayerInventory>();
    //     playerInventory.PickupItem(inventoryItem);

    //     Destroy(item);
    // }

    void Start()
    {
        if (_allowedEvents.Count > 0)
        {
            _amountOfEvents = Random.Range(0, _amountOfEvents + 1);
            if (_amountOfEvents == 0) _amountOfEvents = 1;
            for (int i = 0; i < _amountOfEvents; i++)
                _selectedEventClasses.Add(_allowedEvents[Random.Range(0, _allowedEvents.Count)]);
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
