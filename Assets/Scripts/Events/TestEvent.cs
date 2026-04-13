using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Events/TestEvent")]
    public class TestEvent : EventClass
    {
        //When room spawns in
        public override bool Generate(CarriageClass room)
        {
            Debug.Log("Test Generated");
            return true;
        }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room)
        {
            Debug.Log("Test Approached for first time");
            return true;
        }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room)
        {
            Debug.Log("Test Approached");
            return true;
        }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
        {
            Debug.Log("Test Entered First Time");
            return true;
        }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room)
        {
            Debug.Log("Test Entered");
            return true;
        }
        //First time completing room
        public override bool FirstExit(CarriageClass room)
        {
            Debug.Log("Test Exited First Time");
            return true;
        }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room)
        {
            Debug.Log("Test Exited Early");
            return true;
        }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room)
        {
            Debug.Log("Test Exited");
            return true;
        }
        //Getting far away from the room
        public override bool Recede(CarriageClass room)
        {
            Debug.Log("Test Receded");
            return true;
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            Debug.Log("Test Told to destroy itself");
            return true;
        }
    }

}
