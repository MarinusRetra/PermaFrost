using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Shadowman : Monster
    {
        private void Start()
        {
            //turn off candles, apart from ones in side rooms (so player can figure that out)
            //Move from entrance to exit
            //kill any player nearby
            StartAttack();
        }

        private IEnumerator StartAttack()
        {
            StartCoroutine(CurrentRoom.GetComponent<CandleManager>().FlickerCandles());
            yield return new WaitForSeconds(5);
            transform.position = CurrentRoom.transform.Find("entry").position;
            //??: Do speed up or nah?
        }
    }
}
