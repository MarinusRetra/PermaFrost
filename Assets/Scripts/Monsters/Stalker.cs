using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Stalker : Monster
    {
        private enum StalkerStates { Watching,Watched,Moving,Idle}
        private StalkerStates _currentState = StalkerStates.Watching;

        void Start()
        {
            StartCoroutine(HandleBehaviour());
        }

        private IEnumerator HandleBehaviour()
        {
            while (true)
            {
                switch (_currentState)
                {
                    case StalkerStates.Watching:
                        yield return new WaitForSeconds(0.1f);
                        if (IsPlayerLookingAt.IsPlayerLookingAtObj(GetComponent<Collider>()))
                        {
                            _currentState = StalkerStates.Watched;
                            Attack();
                        }
                        break;
                    case StalkerStates.Watched:
                        //prevent moving
                        if (!IsPlayerLookingAt.IsPlayerLookingAtObj(GetComponent<Collider>()))
                        {
                            _currentState = StalkerStates.Watching;
                            PlayerStatusEffects.Instance.ManageInsanityCauses("Stalker", true);
                        }
                        yield return new WaitForSeconds(0.1f);
                        break;
                    case StalkerStates.Moving:
                        //begone
                        yield return new WaitForSeconds(5f);
                        //beback
                        break;
                }
            }
        }

        private void Attack()
        {
            PlayerStatusEffects.Instance.ManageInsanityCauses("Stalker", false);
        }
    }
}
