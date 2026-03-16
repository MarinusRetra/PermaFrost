using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Shadowman : Monster
    {
        [SerializeField] private float _shadowmanSpeed = 1;

        [SerializeField] private EventClass freezingEvent;

        private bool _hasStarted = false;
        private Transform _exitRoom;
        private Transform _entryRoom;

        [Header("Sounds")]
        [SerializeField] private AudioClip _dashingClip;
        private void Start()
        {
            transform.position = new Vector3(-300, -300, -300);

            _exitRoom = CurrentRoom.Find("Exit");
            _entryRoom = CurrentRoom.Find("Entry");

            //turn off candles, apart from ones in side rooms (so player can figure that out)
            StartCoroutine(StartAttack());
        }

        private IEnumerator StartAttack()
        {
            StartCoroutine(CurrentRoom.GetComponent<CandleManager>().FlickerCandles());

            //makes shadowman appear faster if the room also has freezing, so the player isnt guarenteed to take too much freezing
            yield return new WaitForSeconds(CurrentRoom.GetComponent<CarriageClass>()._selectedEventClasses.Contains(freezingEvent) ?  Random.Range(7f, 10f) : Random.Range(6f, 8.5f));
            
            Soundsystem.PlaySound(_dashingClip, transform.position, true).transform.parent = transform;
            _hasStarted = true;
            transform.position = _entryRoom.position;
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, _shadowmanSpeed), ForceMode.Impulse);

            //Check every half second if its passed
            while (transform.position.z < _exitRoom.position.z + 50)
            {
                yield return new WaitForSeconds(0.5f);
            }
            DestroyMonster();
        }

        private void OnTriggerEnter(Collider other)
        {
            //This covers a triggger on a child, which is about room big
            if (other.CompareTag("Player") && !PlrRefs.inst.PlayerMonsterManager.InSideroom && _hasStarted)
            {
                StartCoroutine(PlrRefs.inst.PlayerHealth.DamagePlayer("Shadowman"));
            }
        }

        public override void DestroyMonster()
        {
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject);
        }
    }
}
