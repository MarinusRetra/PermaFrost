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
    public PlayerInventory playerInventory;

    [SerializeField] private GameObject droppedItemPrefab;

    public void SpawnRandomItem()
    {
        Transform randomLocation = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject newDroppedItem = Instantiate(droppedItemPrefab, randomLocation.position, Quaternion.identity);

        var interactObject = newDroppedItem.GetComponent<InteractObject>();
        if (interactObject != null)
        {
            newDroppedItem.GetComponent<MeshRenderer>().material.color = allowedDrops[Random.Range(0, allowedDrops.Count)].color;
            interactObject._interactEvent.AddListener(() => OnItemInteracted(newDroppedItem));
        }
    }

    private void OnItemInteracted(GameObject item)
    {
        Debug.Log("Player interacted with " + item.name);
    }
}
