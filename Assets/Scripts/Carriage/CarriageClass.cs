using System.Collections.Generic;
using UnityEngine;

public class CarriageClass : MonoBehaviour
{
    [Header("Transforms")]
    public Transform entryPoint;
    public Transform exitPoint;
    public Transform mainObject;

    static public List<Transform> spawnPoints;


    static public void SpawnRandomItem()
    {
        Transform randomLocation = spawnPoints[Random.Range(0, spawnPoints.Count)];
    }
}
