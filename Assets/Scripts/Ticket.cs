using UnityEngine;

namespace Gameplay
{
    public class Ticket : MonoBehaviour
    {
        public void GrabTicket()
        {
            PlayerMonsterManager.Instance.GrabTicket();
        }
    }
}
