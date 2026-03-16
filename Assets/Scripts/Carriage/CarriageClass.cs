using Gameplay;
using System.Collections;
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
    public Generation generationClass;

    [SerializeField] private bool _enterTriggered;
    [SerializeField] private bool _exitTriggered;
    [SerializeField] private bool _triggerTriggered;

    private bool playerInside = false;
    public List<EventClass> _selectedEventClasses;
    [SerializeField] private int _maxAmountOfItems = 1;

    public CarriageClass previousCarriage;

    private PlayerController _player;

    private List<GameObject> spawnedItems = new List<GameObject>();

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
