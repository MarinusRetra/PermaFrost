using UnityEngine;
using System.Collections;

public class TimerListener : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSecondsCached = new(0.2f);

    /// <summary>
    /// Inventory timer gets used to decide of what item it will use the CompleteTimer function
    /// </summary>
    [SerializeField] private InventoryItem _inventoryTimer;
    [SerializeField] private GameObject _bar;
    private Coroutine _currentTimerRoutine;
    private GameObject _lookinAt;

    private float _maxTime;

    private void OnEnable()
    {
        _inventoryTimer.StartTimerEvent += HandleStartTimer;
        _inventoryTimer.CancelledTimerEvent += HandleCancelTimer;
    }

    private void HandleStartTimer(float _timeIn)
    {
        _currentTimerRoutine = StartCoroutine(TimerCoroutine(_timeIn));
        _maxTime = _timeIn;
    }

    private void HandleCancelTimer()
    {
        StopCoroutine(_currentTimerRoutine);
    }

    private IEnumerator TimerCoroutine(float _timeIn)
    {
        for (float i = 0; i <= _timeIn;)
        {
            _lookinAt = Camera.main.GetComponent<Gameplay.Interactor>().hit.collider?.gameObject;
            if (_lookinAt && _lookinAt.CompareTag("Door"))
            {
              i += 0.2f;
              print(i);
            }
            else if(i >= 0)
            {
              i -= 0.1f;
              print(i);
            }
            yield return _waitForSecondsCached;
            UpdateLockPickbar(i);
        }
        _inventoryTimer.CompleteTimer();
    }

    private void OnDisable()
    {
        _inventoryTimer.StartTimerEvent -= HandleStartTimer;
        _inventoryTimer.CancelledTimerEvent -= HandleCancelTimer;
    }
    private void OnDestroy()
    {
        _inventoryTimer.StartTimerEvent -= HandleStartTimer;
        _inventoryTimer.CancelledTimerEvent -= HandleCancelTimer;
    }

    public void UpdateLockPickbar(float progress)
    {
        progress = Mathf.Clamp(progress, 0, _maxTime);
        float normalized = progress / _maxTime;

        Vector3 scale = _bar.transform.localScale;
        scale.x = normalized;
        _bar.transform.localScale = scale;
    }
}
