using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform[] _objects;
    [SerializeField] private Vector3 _speed;
    [SerializeField] private int _distance;
    private float _maxArea;
    private float _minArea;
    void Start()
    {
        _maxArea = _objects[0].transform.position.z + _distance;
        _minArea = _objects[0].transform.position.z - _distance;
    }

    void Update()
    {
        //move the objects
        for(int i = 0; i < _objects.Length; i++)
        {
            _objects[i].transform.Translate(-_speed);
            if (_objects[i].transform.position.z < _minArea)
            {
                _objects[i].transform.position = new Vector3(_objects[i].transform.position.x, _objects[i].transform.position.y,_maxArea);
            }
        }
    }
}
