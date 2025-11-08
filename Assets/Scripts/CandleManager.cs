using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class CandleManager : MonoBehaviour
    {
        public GameObject[] AllCandles;

        public void TurnOffCandles()
        {
            for(int i = 0; i < AllCandles.Length; i++)
            {
                //change when we get actual candles
                AllCandles[i].SetActive(false);
            }
        }

        public void TurnOnCandles()
        {
            for (int i = 0; i < AllCandles.Length; i++)
            {
                //change when we get actual candles
                AllCandles[i].SetActive(true);
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
