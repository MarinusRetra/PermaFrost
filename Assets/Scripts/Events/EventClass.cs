using System;
using UnityEngine;

// [CreateAssetMenu(menuName = "Event/Blank")]
[Serializable]
public class EventClass : ScriptableObject
{
    //On generating the room
    public virtual bool Generate(CarriageClass room) { return false; }
    //On getting near the room for the first time
    public virtual bool FirstApproach(CarriageClass room) { return false; }
    //On getting near the room after the first time
    public virtual bool RepeatApproach(CarriageClass room) { return false; }
    //On first time entering the room
    public virtual bool FirstEnter(CarriageClass room) { return false; }
    //On reentering the room
    public virtual bool RepeatEnter(CarriageClass room) { return false; }
    //On first time leaving the room through the back
    public virtual bool FirstExit(CarriageClass room) { return false; }
    //On leaving the room after having triggered first exit
    public virtual bool RepeatExit(CarriageClass room) { return false; }
    //On leaving the room without having triggered the first exit
    public virtual bool EarlyExit(CarriageClass room) { return false; }
    //On getting far away from the room
    public virtual bool Recede(CarriageClass room) { return false; }
    //Called in various places that makes an event completely destroy everything it made
    public virtual bool CallForDeletion(CarriageClass room) { return false; }
}

public class EventTemplate : EventClass
{
    //Variables

    //When room spawns in
    public override bool Generate(CarriageClass room)
    {
        return true;
    }
    //First time approaching room
    public override bool FirstApproach(CarriageClass room)
    {
        return true;
    }
    //Any other time approaching room
    public override bool RepeatApproach(CarriageClass room)
    {
        return true;
    }
    //First time room entered
    public override bool FirstEnter(CarriageClass room) 
    { 
        return true; 
    }
    //Any other time room entered
    public override bool RepeatEnter(CarriageClass room) 
    { 
        return true; 
    }
    //First time completing room
    public override bool FirstExit(CarriageClass room) 
    { 
        return true; 
    }
    //Leaving room through the way the player came
    public override bool EarlyExit(CarriageClass room) 
    { 
        return true; 
    }
    //Any other time leaving room
    public override bool RepeatExit(CarriageClass room) 
    { 
        return true; 
    }
    //Getting far away from the room
    public override bool Recede(CarriageClass room) 
    { 
        return true; 
    }
    //Removes any evidence of events existance in room
    public override bool CallForDeletion(CarriageClass room) 
    { 
        return true; 
    }
}