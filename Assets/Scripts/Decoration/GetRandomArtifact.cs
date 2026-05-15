using System;
using UnityEngine;

namespace Gameplay
{
    public class GetRandomArtifact : MonoBehaviour
    {
        [SerializeField] private Artifact[] _possibleArtifacts;
        public MeshRenderer rendererer;
        public MeshFilter filterer;
        public MeshRenderer parentRender;
        void Start()
        {
            Artifact chosenArtif = CalculateArtifactWeight(_possibleArtifacts);
            rendererer.materials = chosenArtif.Mats;
            filterer.mesh = chosenArtif.Mesh;
            parentRender.enabled = !chosenArtif.NoGlass;
            transform.localScale = chosenArtif.scale;
        }

        private Artifact CalculateArtifactWeight(Artifact[] Paintings)
        {
            int totalWeight = 0;
            for (int i = 0; i < Paintings.Length; i++)
            {
                totalWeight += Paintings[i].Weight;
            }
            int randomChosenWeight = UnityEngine.Random.Range(1, totalWeight + 1);
            int roomCheckers = 0;
            for (int i = 0; i < Paintings.Length; i++)
            {
                roomCheckers += Paintings[i].Weight;
                if (roomCheckers > randomChosenWeight || roomCheckers == totalWeight)
                {
                    return Paintings[i];
                }
            }
            return new Artifact();
        }
    }

    [Serializable]
    public struct Artifact
    {
        public Material[] Mats;
        public Mesh Mesh;
        public bool NoGlass;
        public int Weight;
        public Vector3 scale;
    }
}
