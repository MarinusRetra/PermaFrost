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
        private List<Light> _allCandles = new List<Light>();
        private List<ParticleSystem> _allCandleParticles = new List<ParticleSystem>();

        private void Start()
        {
            if (_candleHolder)
            {
                _allCandles = _candleHolder.GetComponentsInChildren<Light>().ToList();
                _allCandleParticles = _candleHolder.GetComponentsInChildren<ParticleSystem>().ToList();
            }
            
        }
        public void TurnOffCandles()
        {
            for(int i = 0; i < _allCandles.Count; i++)
            {
                _allCandles[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < _allCandleParticles.Count; i++)
            {
                _allCandleParticles[i].Stop();
            }
        }

        public void TurnOnCandles()
        {
            for (int i = 0; i < _allCandles.Count; i++)
            {
                _allCandles[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < _allCandleParticles.Count; i++)
            {
                _allCandleParticles[i].Play();
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
