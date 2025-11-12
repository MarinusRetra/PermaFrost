using UnityEngine;

public class SprintBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _bar;
    private int _max;

    public void MaxSprint(int max)
    {
        _max = max;
    }
    public void UpdateSprintBar(int _stamina)
    {
        if (_bar == null) return;

        _stamina = Mathf.Clamp(_stamina, 0, _max);
        float normalized = _stamina / (float)_max;

        Vector3 scale = _bar.transform.localScale;
        scale.x = normalized;
        _bar.transform.localScale = scale;
    }
}
