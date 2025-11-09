using UnityEngine;

namespace Gameplay
{
    public class GetRandomPainting : MonoBehaviour
    {
        [SerializeField] private Material[] _possibleMats;
        void Start()
        {
            GetComponent<MeshRenderer>().material = _possibleMats[Random.Range(0,_possibleMats.Length)];
        }
    }
}
