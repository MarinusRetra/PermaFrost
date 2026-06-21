using UnityEngine;

namespace Gameplay
{
    public class PlrRefs : MonoBehaviour
    {
        public static PlrRefs inst;
        public PlayerHealth PlayerHealth;
        public PlayerStatusEffects PlayerStatusEffects;
        public PlayerMonsterManager PlayerMonsterManager;
        public FreezingLantern FreezingLantern;
        public PlayerInventory PlayerInventory;
        public PlayerController PlayerController;
        public Interactor Interactor;
        public GameManager GameManager;
        public Camera Camera;
        public GameObject PlayerCamera;
        public Camera FakeCamera;
        public Camera PlayerArmCamera;
        public GameObject PlayerArm;
        public Animator PlayerArmAnimator;
        void Awake()
        {
            inst = this;
        }
    }
}
