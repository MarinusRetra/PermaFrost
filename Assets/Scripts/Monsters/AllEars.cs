using UnityEngine;

namespace Gameplay
{
    public class AllEars : Monster
    {
        private GameObject _player;
        [SerializeField] private float _playerRadius;

        [SerializeField] private float _wallRadius;

        [SerializeField] private float _lastPosRadius;
        [SerializeField] private Vector3 _lastPosition;

        public Transform CurrentRoom;
        [SerializeField] private Vector2[] _roomCorners;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Fetch the player
            _player = FindAnyObjectByType<PlayerHealth>().gameObject;

            //Get the rooms corners
            Vector3 _boundingCorner1 = CurrentRoom.transform.position - (CurrentRoom.transform.lossyScale / 2);
            Vector3 _boundingCorner2 = CurrentRoom.transform.position + (CurrentRoom.transform.lossyScale / 2);

            _roomCorners = new Vector2[]{ new Vector2(_boundingCorner1.x + _wallRadius, _boundingCorner1.z + _wallRadius),new Vector2(_boundingCorner2.x - _wallRadius, _boundingCorner2.z - _wallRadius)};
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
