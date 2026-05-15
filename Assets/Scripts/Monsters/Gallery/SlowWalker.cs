using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class SlowWalker : Monster
    {
        [SerializeField] private float _shadowmanSpeed = 1;

        [SerializeField] private NodeBasedMovement movement;
        private void Start()
        {
            
        }

        public void StartAttack()
        {

            movement.speed = _shadowmanSpeed;

            CarriageClass carriageBack = CurrentRoom.GetComponent<CarriageClass>();
            transform.position = carriageBack.EntryPoint.position;

            movement.StartMoving(carriageBack.NodeHolder);
            movement.onDeathAction = () => { DestroyMonster(); };
        }

        private void OnTriggerEnter(Collider other)
        {
            //This covers a triggger on a child, which is about room big
            if (other.CompareTag("Player"))
            {
                StartCoroutine(PlrRefs.inst.PlayerHealth.DamagePlayer("SlowWalker"));
            }
        }

        public override void DestroyMonster()
        {
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject);
        }
    }
}
