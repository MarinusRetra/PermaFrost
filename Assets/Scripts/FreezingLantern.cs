using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class FreezingLantern : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        public bool LanternOn;
        public static float _range = 10;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _input.LanternEvent += ChangeLanternState;
            StartCoroutine(HandleLantern());
        }

        private void ChangeLanternState()
        {
            LanternOn = !LanternOn;
            for(int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(LanternOn);
            }
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
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, _range);
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

        private void OnDestroy()
        {
            _input.LanternEvent -= ChangeLanternState;
        }

        private void OnDisable()
        {
            _input.LanternEvent -= ChangeLanternState;
        }
    }
}
