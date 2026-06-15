using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class StandinInPlate : MonoBehaviour
    {
        [Header("Customization Values")]

        [Tooltip("Put whatever you want to activate after the player stood in the area for a while")]
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
                //Add time
                CurrentTime += Time.deltaTime;

                //If we at enough time: Activate EnoughTimeInArea();
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
            //So it dont activate again
            WasActivated = true;

            //Does the unity event
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
            //Add more if needed
            if (!WasActivated)
            {
                PlayerInArea = true;
            }
        }

        private void OnPlayerExit()
        {
            //Add more if needed
            if (!WasActivated)
            {
                PlayerInArea = false;
            }
        }
    }
}
