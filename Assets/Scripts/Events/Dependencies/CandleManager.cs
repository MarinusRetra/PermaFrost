using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Gameplay
{
    public class CandleManager : MonoBehaviour
    {
        public GameObject _candleHolder;
        [SerializeField] private Light [] _allCandles;
        [SerializeField] private ParticleSystem[] _allCandleParticles;
        [SerializeField] private UniversalAdditionalLightData[] _allCandleLightData;

        public bool beenCalled = false;

        /// <summary>
        /// backup function, set candles through editor for better optimization.
        /// </summary>
        private void SetupCandles()
        {
            Debug.LogWarning(gameObject.name + " Room forced to setup candles itself, Use the Testing Utils/Player Editor to set this trough the editor for optimization");
            if (_candleHolder)
            {
                _allCandles = _candleHolder.GetComponentsInChildren<Light>().ToArray();
                _allCandleParticles = _candleHolder.GetComponentsInChildren<ParticleSystem>().ToArray();
                beenCalled = true;
            }

        }
        public void TurnOffCandles()
        {
            if (!beenCalled) { SetupCandles(); }
            for(int i = 0; i < _allCandles.Length; i++)
            {
                _allCandles[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < _allCandleParticles.Length; i++)
            {
                _allCandleParticles[i].Stop();
            }
        }

        public void TurnOnCandles()
        {
            if (!beenCalled) { SetupCandles(); }
            for (int i = 0; i < _allCandles.Length; i++)
            {
                _allCandles[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < _allCandleParticles.Length; i++)
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
        public void SetCandleLists(Light[] lights, ParticleSystem[] particles, UniversalAdditionalLightData[] lightData)
        {
            _allCandleParticles = particles;
            _allCandles = lights;
            _allCandleLightData = lightData;
            beenCalled = true;
        }
#endif
    }
}
