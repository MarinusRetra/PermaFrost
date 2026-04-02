using System;
using UnityEngine;
[CreateAssetMenu(menuName = "Item/Item")]
[Serializable]
public class InventoryItem : ScriptableObject
{
    public Sprite sprite = null;
    public Color color = new(1, 1, 1, 255);
    public GameObject HoldObject = null;
    public event Action<float> StartTimerEvent;
    public event Action CancelledTimerEvent;

    /// <summary>
    /// If the functions returns true that means the use was succesfull and it will be taken out of the hotbar. 
    /// </summary>
    public virtual bool Use() { return false; }

    /// <summary>
    /// What happens when you release the use button
    /// </summary>
    public virtual void UseCancelled() { CancelledTimerEvent?.Invoke(); }

    /// <summary>
    /// What happens when a timer called from an item finishes
    /// </summary>
    public virtual void CompleteTimer() { }

    public void StartTimer(float _timeIn)
    {
        StartTimerEvent.Invoke(_timeIn);
    }
}




