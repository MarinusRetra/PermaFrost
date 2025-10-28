using System.Collections.Generic;
using UnityEngine;

public class CarriageClass : MonoBehaviour
{
    [Header("Transforms")]
    public Transform entryPoint;
    public Transform exitPoint;
    public Transform mainObject;

    public List<Transform> spawnPoints;
    public List<InventoryItem> allowedDrops;

    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private Transform dropParent;


    public void SpawnRandomItem()
    {
        Transform randomLocation = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject newDroppedItem = Instantiate(droppedItemPrefab, dropParent);

    }
}
