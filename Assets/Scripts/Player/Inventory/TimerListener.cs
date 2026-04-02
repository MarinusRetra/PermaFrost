using UnityEngine;
using System.Collections;
using Gameplay;

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
        _bar.transform.parent.gameObject.SetActive(true);
    }

    private void HandleCancelTimer()
    {
        StopCoroutine(_currentTimerRoutine);
        _bar.transform.parent.gameObject.SetActive(false);
    }

    private IEnumerator TimerCoroutine(float _timeIn)
    {
        for (float i = 0; i <= _timeIn;)
        {
            _lookinAt = PlrRefs.inst.Interactor.hit.collider?.gameObject;
            if (_lookinAt && _lookinAt.CompareTag("Door"))
            {
              i += 0.3f;
            }
            else if(i >= 0)
            {
              i -= 0.1f;
            }
            yield return _waitForSecondsCached;
            UpdateLockPickbar(i);
        }
        _inventoryTimer.CompleteTimer();
        _bar.transform.parent.gameObject.SetActive(false);
    }

    private void UpdateLockPickbar(float _progress)
    {
        _progress = Mathf.Clamp(_progress, 0, _maxTime);
        float normalized = _progress / _maxTime;

        Vector3 scale = _bar.transform.localScale;
        scale.x = normalized;
        _bar.transform.localScale = scale;
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
}
