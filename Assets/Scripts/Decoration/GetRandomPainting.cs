using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class GetRandomPainting : MonoBehaviour
    {
        [SerializeField] private Painting[] _possiblePaintings;
        [SerializeField] private MeshRenderer frameFilter;
        [SerializeField] private MeshRenderer PaintingRenderer;
        void Start()
        {
            Painting chosenPainting = CalculatePaintingWeight(_possiblePaintings);
            PaintingRenderer.material = chosenPainting.Mat;
            if (chosenPainting.RemoveFrame) { frameFilter.enabled = false; }
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
