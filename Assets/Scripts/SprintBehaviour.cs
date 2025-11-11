using UnityEngine;

public class SprintBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _bar;
    public void UpdateSprintBar(int _stamina)
    {
        if (_bar == null) return;

        _stamina = Mathf.Clamp(_stamina, 0, 35);
        float normalized = _stamina / 35f;

        Vector3 scale = _bar.transform.localScale;
        scale.x = normalized;
        _bar.transform.localScale = scale;
    }
}
