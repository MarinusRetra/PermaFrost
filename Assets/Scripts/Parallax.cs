using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform[] _objects;
    [SerializeField] private Vector3 _speed;
    [SerializeField] private int distance;
    private float maxArea;
    private float minArea;
    void Start()
    {
        maxArea = _objects[0].transform.position.z + distance;
        minArea = _objects[0].transform.position.z - distance;
    }

    void Update()
    {
        //move the objects
        for(int i = 0; i < _objects.Length; i++)
        {
            _objects[i].transform.Translate(-_speed);
            if (_objects[i].transform.position.z < minArea)
            {
                _objects[i].transform.position = new Vector3(_objects[i].transform.position.x, _objects[i].transform.position.y,maxArea);
            }
        }
    }
}
