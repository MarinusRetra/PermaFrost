using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class FreezingLantern : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        public bool LanternOn;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(HandleLantern());
        }

        public List<GameObject> _objectsInAreaRightNow = new List<GameObject>();
        public List<GameObject> _objectsInAreaLastCheck = new List<GameObject>();
        private IEnumerator HandleLantern()
        {
            yield return new WaitForSeconds(1);
            while(true)
            {
                if (LanternOn)
                {
                    _objectsInAreaRightNow.Clear();

                    //Get all the objects in the area
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2);
                    foreach (var hitCollider in hitColliders)
                    {
                        if (_objectsInAreaRightNow.Contains(hitCollider.gameObject)) continue;

                        //Remove object from list if its still here
                        if (_objectsInAreaLastCheck.Contains(hitCollider.gameObject)) { _objectsInAreaLastCheck.Remove(hitCollider.gameObject); }
                        
                        bool wasChanged = AffectObject(hitCollider.gameObject,true);
                        if(wasChanged) _objectsInAreaRightNow.Add(hitCollider.gameObject);
                    }

                    //Unfreeze any object that wasnt removed from the list
                    foreach(GameObject unfreezingObject in _objectsInAreaLastCheck)
                    {
                        AffectObject(unfreezingObject, false);
                        
                    }
                    _objectsInAreaLastCheck.Clear();
                    _objectsInAreaLastCheck = new List<GameObject>(_objectsInAreaRightNow);
                    
                }
                else
                {
                    //Remove frostbite whenever the lantern is off
                    PlayerStatusEffects.Instance.ManageFrostbiteCauses("Lantern", true);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void ChangeLanternState(bool on)
        {
            if (on)
            {
                LanternOn = true;
                //turn on lantern idk
            }
            else
            {
                LanternOn= false;
            }
        }

        /// <summary>
        /// Check if object is affected by the lantern, and affects then if they are.
        /// </summary>
        /// <param name="objec"></param>
        /// <param name="turnOn"></param>
        /// <returns></returns>
        private bool AffectObject(GameObject objec, bool turnOn)
        {
            if (objec.CompareTag("Player"))
            {
                PlayerStatusEffects.Instance.ManageFrostbiteCauses("Lantern", !turnOn);
                print("Player is freezing due to the lantern");
                return true;
            }

            if (objec.GetComponent<HotDude>())
            {
                objec.GetComponent<HotDude>().Stun();
                return true;
            }
            return false;
        }
    }
}
