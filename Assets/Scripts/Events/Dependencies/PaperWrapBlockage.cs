using UnityEngine;

namespace Gameplay
{
    public class PaperWrapBlockage : MonoBehaviour
    {
        public Collider MainCollder;
        [SerializeField] private Animator WrapAnimator;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            WrapAnimator.enabled = true;
        }

        public void Unwrap()
        {
            MainCollder.enabled = false;
            WrapAnimator.SetTrigger("Open");
        }
    }
}
