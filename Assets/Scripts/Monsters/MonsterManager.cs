using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class MonsterManager : MonoBehaviour
    {
        /// <summary>
        /// Fill in a location for them to go there
        /// </summary>
        /// <param name="location"></param>
        public static void MakeNoise(Vector3 location)
        {
            var allEars = FindObjectsByType<AllEars>(FindObjectsSortMode.None);
            for (int i = 0; i < allEars.Length; i++)
            {
                allEars[i].Aggro(location);
            }
        }

        /// <summary>
        /// Defaults to player
        /// </summary>
        public static void MakeNoise()
        {
            var allEars = FindObjectsByType<AllEars>(FindObjectsSortMode.None);
            for (int i = 0; i < allEars.Length; i++)
            {
                allEars[i].Aggro(FindAnyObjectByType<PlayerHealth>().transform.position);
            }
        }
    }
}
