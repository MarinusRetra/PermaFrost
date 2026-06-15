using UnityEngine;

namespace Gameplay
{
    public class PaperWrapBlockage : MonoBehaviour
    {
        public Collider MainCollder;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //do animation :D
        }

        public void Unwrap()
        {
            MainCollder.enabled = false;
            //do other animation :D
        }
    }
}
