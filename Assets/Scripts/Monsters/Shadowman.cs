using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Shadowman : Monster
    {
        [SerializeField] private float _shadowmanSpeed = 1;
        private Transform _exitRoom;
        private Transform _entryRoom;

        [SerializeField] private AudioClip _dashingClip;

        private bool _hasStarted = false;
        private void Start()
        {
            _exitRoom = CurrentRoom.Find("Exit");
            _entryRoom = CurrentRoom.Find("Entry");
            //turn off candles, apart from ones in side rooms (so player can figure that out)
            StartCoroutine(StartAttack());
        }

        private IEnumerator StartAttack()
        {
            StartCoroutine(CurrentRoom.GetComponent<CandleManager>().FlickerCandles());
            yield return new WaitForSeconds(5);
            GameObject _noises = Soundsystem.PlaySound(_dashingClip, transform.position, true);
            _noises.transform.parent = transform;
            _hasStarted = true;
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            transform.position = _entryRoom.position;
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, _shadowmanSpeed), ForceMode.Impulse);

            while (transform.position.z < _exitRoom.position.z + 20)
            {
                yield return new WaitForSeconds(0.5f);
            }
            DestroyMonster();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !PlayerMonsterManager.Instance.InSideroom && _hasStarted)
            {
                StartCoroutine(PlayerHealth.Instance.DamagePlayer());
            }
        }

        public override void DestroyMonster()
        {
            print("Shadowman despawning");
            GetComponent<Collider>().enabled = false;
            StartCoroutine(DespawnAnim());
        }

        private IEnumerator DespawnAnim()
        {
            Transform model = transform.GetChild(0);
            while (model.localPosition.y > -7)
            {
                model.localPosition = new Vector3(model.localPosition.x, model.localPosition.y - 0.2f, model.localPosition.z);
                yield return new WaitForSeconds(0.05f);
            }
            Destroy(gameObject);
        }
    }
}
