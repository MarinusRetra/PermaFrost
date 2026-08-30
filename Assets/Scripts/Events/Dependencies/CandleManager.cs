using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class CandleManager : MonoBehaviour
    {
        public GameObject _candleHolder;
        [SerializeField] private List<Light> _allCandles = new List<Light>();
        [SerializeField] private List<ParticleSystem> _allCandleParticles = new List<ParticleSystem>();

        public bool beenCalled = false;

        /// <summary>
        /// backup function, set candles through editor for better optimization.
        /// </summary>
        private void SetupCandles()
        {
            Debug.LogWarning(gameObject.name + " Room forced to setup candles itself, Use the Testing Utils/Player Editor to set this trough the editor for optimization");
            if (_candleHolder)
            {
                _allCandles = _candleHolder.GetComponentsInChildren<Light>().ToList();
                _allCandleParticles = _candleHolder.GetComponentsInChildren<ParticleSystem>().ToList();
                beenCalled = true;
            }

        }
        public void TurnOffCandles()
        {
            if (!beenCalled) { SetupCandles(); }
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
            if (!beenCalled) { SetupCandles(); }
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

#if UNITY_EDITOR
        public void SetCandleLists(List<Light> lights, List<ParticleSystem> particles)
        {
            _allCandleParticles = particles;
            _allCandles = lights;
            beenCalled = true;
        }
#endif
    }
}
