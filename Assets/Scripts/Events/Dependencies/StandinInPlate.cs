using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class StandinInPlate : MonoBehaviour
    {
        [Header("Customization Values")]

        [Tooltip("This event is called after the player has stoon in the area for {TimeBeforeUse} seconds")]
        [SerializeField] private UnityEvent OnEnoughTimeIn;
        [SerializeField] private float TimeBeforeUse;

        [Header("References")]
        public PaperWrapBlockage WrapBlockage;

        //Changing values
        private float CurrentTime;
        private bool PlayerInArea = false;
        private bool WasActivated = false;

        void Start()
        {
            //Do animation :D
            CurrentTime = 0;
        }

        void Update()
        {
            if (WasActivated) { return; }

            if (PlayerInArea)
            {
                CurrentTime += Time.deltaTime;

                if (CurrentTime > TimeBeforeUse) { EnoughTimeInArea(); }
            }
            else
            {
                if(CurrentTime > 0)
                {
                    CurrentTime -= (Time.deltaTime * 2);
                }
                else { CurrentTime = 0; }
                //Remove time
            }
            //Update sprite based on progress :D
        }

        private void EnoughTimeInArea()
        {
            WasActivated = true;

            OnEnoughTimeIn.Invoke();

            WrapBlockage.Unwrap();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                OnPlayerEnter();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                OnPlayerExit();
            }
        }

        private void OnPlayerEnter()
        {
            if (!WasActivated)
            {
                PlayerInArea = true;
            }
        }

        private void OnPlayerExit()
        {
            if (!WasActivated)
            {
                PlayerInArea = false;
            }
        }
    }
}
