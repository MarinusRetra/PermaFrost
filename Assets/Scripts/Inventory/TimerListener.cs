using UnityEngine;
using System.Collections;

public class TimerListener : MonoBehaviour
{
    /// <summary>
    /// Inventory timer gets used to decide of what item it will use the CompleteTimer function
    /// </summary>
    [SerializeField] private InventoryItem _inventoryTimer;
    Coroutine currentTimerRoutine;
    private void OnEnable()
    {
        _inventoryTimer.StartTimerEvent += HandleStartTimer;
        _inventoryTimer.CancelledTimerEvent += HandleCancelTimer;
    }

    private void HandleStartTimer(float _timeIn)
    {
        currentTimerRoutine = StartCoroutine(TimerCoroutine(_timeIn));
    }

    private void HandleCancelTimer()
    {
        StopCoroutine(currentTimerRoutine);
    }

    private IEnumerator TimerCoroutine(float _timeIn)
    {
        yield return new WaitForSeconds(_timeIn);
        _inventoryTimer.CompleteTimer();
    }

    private void OnDisable()
    {
        _inventoryTimer.StartTimerEvent -= HandleStartTimer;
        _inventoryTimer.CancelledTimerEvent -= HandleCancelTimer;
    }
    private void Oestroy()
    {
        _inventoryTimer.StartTimerEvent -= HandleStartTimer;
        _inventoryTimer.CancelledTimerEvent -= HandleCancelTimer;
    }
}
