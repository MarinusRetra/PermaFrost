using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class CarriageClass : MonoBehaviour
{
    [Header("Transforms")]
    public Transform entryPoint;
    public Transform exitPoint;
    public Transform mainObject;
    public Transform spawnPoint;

    public List<Transform> spawnPoints;
    public List<InventoryItem> allowedDrops;
    public Generation generationClass;

    [SerializeField] private GameObject droppedItemPrefab;

    public void SpawnRandomItem()
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

    private void OnItemInteracted(GameObject item, InventoryItem inventoryItem)
    {
        Debug.Log("Player interacted with " + item.name);

        PlayerInventory playerInventory = generationClass.player.GetComponent<PlayerInventory>();
        playerInventory.PickupItem(inventoryItem);

        Destroy(item);
    }
}
