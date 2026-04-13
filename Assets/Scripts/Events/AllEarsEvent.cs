using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/AllEarsEvent")]
    public class AllEarsEvent : EventClass
    {
        [SerializeField] private GameObject _allEarsPrefab;
        private GameObject _spawnedAllEars;

        //When room spawns in
        public override bool Generate(CarriageClass room) { return true; }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room)
        {
            _spawnedAllEars = Instantiate(_allEarsPrefab);
            _spawnedAllEars.transform.parent = room.Holder;
            _spawnedAllEars.GetComponent<Monster>().CurrentRoom = room.transform;
            _spawnedAllEars.transform.localPosition = new Vector3(0, 0, 0);
            _spawnedAllEars.GetComponent<NavMeshAgent>().enabled = true;
            _spawnedAllEars.GetComponent<AllEars>().Start();
            _spawnedAllEars.GetComponent<AllEars>().SetIdleState(true);
            return true;
        }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room)
        {
            _spawnedAllEars = room.Holder.Find("AllEars(Clone)").gameObject;
            _spawnedAllEars.SetActive(true);
            _spawnedAllEars.GetComponent<AllEars>().Start();
            _spawnedAllEars.GetComponent<AllEars>().SetIdleState(true);
            return true;
        }
        //First time room entered
        public override bool FirstEnter(CarriageClass room) { return RepeatEnter(room); }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room)
        {
            room.Holder.Find("AllEars(Clone)").GetComponent<AllEars>().SetIdleState(false);
            return true;
        }
        //First time completing room
        public override bool FirstExit(CarriageClass room) { return RepeatExit(room); }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room) { return RepeatExit(room); }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room)
        {
            room.Holder.Find("AllEars(Clone)").GetComponent<AllEars>().SetIdleState(true);
            return true;
        }
        //Getting far away from the room
        public override bool Recede(CarriageClass room)
        {
            room.Holder.Find("AllEars(Clone)")?.gameObject.SetActive(false);
            return true;
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            room.Holder.Find("AllEars(Clone)").GetComponent<Monster>().DestroyMonster();
            return true;
        }
    }
}
