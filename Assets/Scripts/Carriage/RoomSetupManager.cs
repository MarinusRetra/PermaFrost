using System;
using UnityEngine;

namespace Gameplay
{
    public class RoomSetupManager : MonoBehaviour
    {
        public RoomVariation[] variations;

        public void ApplyVariant()
        {
            if (variations == null) { return; }
            RoomVariation chosenVariant = new RoomVariation();
            if (variations.Length > 1)
            {
                chosenVariant = ChooseVariant();
            }
            else { return; }

            if (chosenVariant == variations[0]) { return; }
            ActuallyApplyVariant(chosenVariant);
            gameObject.name += "-" + chosenVariant.Name;
        }

        public void ActuallyApplyVariant(RoomVariation chosenVariant)
        {

            for (int i = 0; i < variations[0].ObjectsToTurnOn.Length; i++)
            {
                variations[0].ObjectsToTurnOn[i].SetActive(false);
            }

            for (int i = 0; i < chosenVariant.ObjectsToTurnOn.Length; i++)
            {
                chosenVariant.ObjectsToTurnOn[i].SetActive(true);
            }
        }

        public void TurnOffEverything()
        {
            for(int i = 0; i < variations.Length; i++)
            {
                for (int j = 0; j < variations[i].ObjectsToTurnOn.Length; j++)
                {
                    variations[i].ObjectsToTurnOn[j].SetActive(false);
                }

            }
        }

        private RoomVariation ChooseVariant()
        {
            int totalWeight = 0;
            for (int i = 0; i < variations.Length; i++)
            {
                totalWeight += variations[i].Weight;
            }
            int randomChosenWeight = UnityEngine.Random.Range(1, totalWeight + 1);
            int roomCheckers = 0;
            for (int i = 0; i < variations.Length; i++)
            {
                roomCheckers += variations[i].Weight;
                if (roomCheckers > randomChosenWeight || roomCheckers == totalWeight)
                {
                    return variations[i];
                }
            }
            return variations[0];
        }
    }

    [Serializable]
    public class RoomVariation
    {
        public GameObject[] ObjectsToTurnOn;
        public int Weight;
        public string Name;
    }
}
