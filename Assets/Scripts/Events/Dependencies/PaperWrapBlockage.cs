using UnityEngine;

namespace Gameplay
{
    public class PaperWrapBlockage : MonoBehaviour
    {
        public Collider MainCollder;
        [SerializeField] private Animator WrapAnimator;
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
