using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    public class LightsOutEvent : EventClass
    {
        public bool lightsout = false;
        //When room spawns in
        public override bool Generate(CarriageClass room) 
        {
            return true; 
        }
        //First time approaching room
        public override bool FirstApproach(CarriageClass room) 
        {
            bool breakEarly = Random.Range(0, 3) > 1;
            if (breakEarly)
            {
                LightsOut(room);
                lightsout = true;
            }
            return true; 
        }
        //Any other time approaching room
        public override bool RepeatApproach(CarriageClass room) { return true; }
        //First time room entered
        public override bool FirstEnter(CarriageClass room)
        {
            if (!lightsout)
            {
                LightsOut(room);
                lightsout = true;
            }
            return true;
        }
        //Any other time room entered
        public override bool RepeatEnter(CarriageClass room) { return true; }
        //First time completing room
        public override bool FirstExit(CarriageClass room)
        {
            return true;
        }
        //Leaving room through the way the player came
        public override bool EarlyExit(CarriageClass room) { return true; }
        //Any other time leaving room
        public override bool RepeatExit(CarriageClass room) { return true; }
        //Getting far away from the room
        public override bool Recede(CarriageClass room) 
        {
            lightsout = false;
            return true; 
        }
        //Removes any evidence of events existance in room
        public override bool CallForDeletion(CarriageClass room)
        {
            return true;
        }

        public void LightsOut(CarriageClass room)
        {
            room.GetComponent<CandleManager>().TurnOffCandles();
        }
    }
}
