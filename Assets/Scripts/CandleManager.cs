using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class CandleManager : MonoBehaviour
    {
        [SerializeField] private GameObject _candleHolder;
        private List<ParticleSystem> _allCandles;

        private void Start()
        {
            _allCandles = _candleHolder.GetComponentsInChildren<ParticleSystem>().ToList();
            
        }
        public void TurnOffCandles()
        {
            for(int i = 0; i < _allCandles.Count; i++)
            {
                _allCandles[i].Stop();
                _allCandles[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        public void TurnOnCandles()
        {
            for (int i = 0; i < _allCandles.Count; i++)
            {
                _allCandles[i].Play();
                _allCandles[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        public IEnumerator FlickerCandles()
        {
            TurnOffCandles();
            yield return new WaitForSeconds(3f);
            TurnOnCandles();
            yield return new WaitForSeconds(0.5f);
            TurnOffCandles();
            yield return new WaitForSeconds(3f);
            TurnOnCandles();
        }
    }
}
