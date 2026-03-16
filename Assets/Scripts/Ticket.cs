using UnityEngine;

namespace Gameplay
{
    public class Ticket : MonoBehaviour
    {
        public void GrabTicket()
        {
            PlrRefs.inst.PlayerMonsterManager.GrabTicket();
        }
    }
}
