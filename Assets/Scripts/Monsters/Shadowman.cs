using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Shadowman : Monster
    {
        [SerializeField] private float _shadowmanSpeed = 1;
        private Transform _exitRoom;
        private Transform _entryRoom;
        private void Start()
        {
            _exitRoom = CurrentRoom.Find("exit");
            _entryRoom = CurrentRoom.Find("entry");
            //turn off candles, apart from ones in side rooms (so player can figure that out)
            StartCoroutine(StartAttack());
        }

        private IEnumerator StartAttack()
        {
            StartCoroutine(CurrentRoom.GetComponent<CandleManager>().FlickerCandles());
            yield return new WaitForSeconds(5);
            transform.position = _entryRoom.position;
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, _shadowmanSpeed), ForceMode.Impulse);

            while (transform.position.z < _exitRoom.position.z + 20)
            {
                yield return new WaitForSeconds(0.5f);
            }
            print("Shadowman despawning");
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !PlayerMonsterManager.Instance.InSideroom)
            {
                StartCoroutine(PlayerHealth.Instance.DamagePlayer());
            }
        }
    }
}
