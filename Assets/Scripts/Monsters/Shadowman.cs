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

        [SerializeField] private NodeBasedMovement movement;

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

            CarriageClass currentCarriage = CurrentRoom.GetComponent<CarriageClass>();
            //makes shadowman appear faster if the room also has freezing, so the player isnt guarenteed to take too much freezing
            yield return new WaitForSeconds(currentCarriage._selectedEventClasses.Contains(freezingEvent) ?  Random.Range(7f, 10f) : Random.Range(6f, 8.5f));
            
            Soundsystem.PlaySound(_dashingClip, transform.position, true).transform.parent = transform;
            _hasStarted = true;

            movement.speed = _shadowmanSpeed;

            CarriageClass carriageBack = null;
            if(currentCarriage.roomIndex <= 3)
            {
                carriageBack = currentCarriage.generationClass._initializedRooms[1].GetComponent<CarriageClass>();
            }
            else
            {
                carriageBack = currentCarriage.generationClass._initializedRooms[currentCarriage.roomIndex - 3].GetComponent<CarriageClass>();
            }

            movement.StartMoving(carriageBack.NodeHolder);
            movement.onDeathAction = () => { DestroyMonster(); };
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
