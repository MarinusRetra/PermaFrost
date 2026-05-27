using UnityEngine;

namespace Gameplay
{
    public class InstantDeath : MonoBehaviour
    {
        public string DeathName = "Unknown";
        public void InstantPlayerDeath()
        {
            PlrRefs.inst.PlayerHealth.GameOver(DeathName);
        }
    }
}
