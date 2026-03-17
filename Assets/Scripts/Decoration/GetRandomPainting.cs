using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class GetRandomPainting : MonoBehaviour
    {
        [SerializeField] private Material[] _possibleMats;
        [SerializeField] private Painting[] _possiblePaintings;
        void Start()
        {
            //GetComponent<MeshRenderer>().material = _possibleMats[Random.Range(0,_possibleMats.Length)];
            GetComponent<MeshRenderer>().material = CalculatePaintingWeight(_possiblePaintings).Mat;
        }

        private Painting CalculatePaintingWeight(Painting[] Paintings)
        {
            int totalWeight = 0;
            for (int i = 0; i < Paintings.Length; i++)
            {
                totalWeight += Paintings[i].Weight;
            }
            int randomChosenWeight = Random.Range(1, totalWeight + 1);
            int roomCheckers = 0;
            for (int i = 0; i < Paintings.Length; i++)
            {
                roomCheckers += Paintings[i].Weight;
                if (roomCheckers > randomChosenWeight || roomCheckers == totalWeight)
                {
                    return Paintings[i];
                }
            }
            return new Painting();
        }
    }
}
