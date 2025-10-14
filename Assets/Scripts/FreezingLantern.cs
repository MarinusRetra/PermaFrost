using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class FreezingLantern : MonoBehaviour
    {
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
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2);
                    foreach (var hitCollider in hitColliders)
                    {
                        if (_objectsInAreaRightNow.Contains(hitCollider.gameObject)) continue;
                        if (_objectsInAreaLastCheck.Contains(hitCollider.gameObject)) { _objectsInAreaLastCheck.Remove(hitCollider.gameObject); }
                        bool wasChanged = AffectObject(hitCollider.gameObject,true);
                        if(wasChanged) _objectsInAreaRightNow.Add(hitCollider.gameObject);
                    }
                    foreach(GameObject unfreezingObject in _objectsInAreaLastCheck)
                    {
                        AffectObject(unfreezingObject, false);
                        
                    }
                    _objectsInAreaLastCheck.Clear();
                    _objectsInAreaLastCheck = new List<GameObject>(_objectsInAreaRightNow);
                    
                }
                else
                {
                    PlayerStatusEffects.Instance.ManageFrostbiteCauses("Lantern", true);
                }
                yield return new WaitForSeconds(1f);
            }
        }

        private bool AffectObject(GameObject objec, bool turnOn)
        {
            if (objec.CompareTag("Player"))
            {
                PlayerStatusEffects.Instance.ManageFrostbiteCauses("Lantern", !turnOn);
                print("freeze or something idk man");
                return true;
            }

            if (objec.GetComponent<HotDude>())
            {
                objec.GetComponent<HotDude>().Stun();
            }
            return false;
        }
    }
}
