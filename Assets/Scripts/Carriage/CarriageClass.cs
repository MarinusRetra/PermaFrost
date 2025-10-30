using System.Collections.Generic;
using Gameplay;
using UnityEditor.PackageManager;
using UnityEngine;

public class CarriageClass : MonoBehaviour
{
    [Header("Transforms")]
    public Transform entryPoint;
    public Transform exitPoint;
    public Transform mainObject;
    public Transform spawnPoint;

    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<InventoryItem> allowedDrops;
    [SerializeField] private List<EventClass> allowedEvents;
    public Generation generationClass;

    [SerializeField] private bool EnterTriggered;
    [SerializeField] private bool ExitTriggered;
    [SerializeField] private bool TriggerTriggered;

    private bool playerInside = false;

    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private EventClass selectedEventClass;
    

    public void SpawnRandomItem()
    {
        if (spawnPoints.Count > 0)
        {
            Transform randomLocation = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject newDroppedItem = Instantiate(droppedItemPrefab, randomLocation.position, Quaternion.identity);

            var interactObject = newDroppedItem.GetComponent<InteractObject>();
            if (interactObject != null)
            {
                InventoryItem inventoryItem = allowedDrops[Random.Range(0, allowedDrops.Count)];
                newDroppedItem.GetComponent<MeshRenderer>().material.color = inventoryItem.color;
                interactObject._interactEvent.AddListener(() => OnItemInteracted(newDroppedItem, inventoryItem));
            }
        }
    }

    private void OnItemInteracted(GameObject item, InventoryItem inventoryItem)
    {
        Debug.Log("Player interacted with " + item.name);

        PlayerInventory playerInventory = generationClass.player.GetComponent<PlayerInventory>();
        playerInventory.PickupItem(inventoryItem);

        Destroy(item);
    }

    void Start()
    {
        if (allowedEvents.Count > 0)
        {
            selectedEventClass = allowedEvents[Random.Range(0, allowedEvents.Count)];
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && !playerInside && selectedEventClass)
        {
            playerInside = true;
            // Add your logic here
            selectedEventClass.Entered();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3 && playerInside && selectedEventClass)
        {
            playerInside = false;
            // Add your logic here
            selectedEventClass.Exited();
        }
    }
}
