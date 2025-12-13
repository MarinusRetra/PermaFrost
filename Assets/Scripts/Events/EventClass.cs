using System;
using UnityEngine;

// [CreateAssetMenu(menuName = "Event/Blank")]
[Serializable]
public class EventClass : ScriptableObject
{
    public virtual bool Entered(CarriageClass room) { return false; }
    public virtual bool Exited(CarriageClass room) { return false; }
    public virtual bool Triggered(CarriageClass room) { return false; }
    public virtual bool Generated(CarriageClass room) { return false; }
    //practically the opposite of generated, removes things that dont get removed on exit
    public virtual bool CallForDeletion(CarriageClass room) { return false; }
}

[CreateAssetMenu(menuName = "Events/TestEvent")]
public class TestEvent : EventClass
{
    public override bool Entered(CarriageClass room)
    {
        Debug.Log("Test Entered");
        return true;
    }
    public override bool Exited(CarriageClass room)
    {
        Debug.Log("Test Exited");
        return true;
    }
    public override bool Triggered(CarriageClass room)
    {
        Debug.Log("Test Triggered");
        return true;
    }
    public override bool Generated(CarriageClass room) { return true; }
    public override bool CallForDeletion(CarriageClass room) { return true; }
}

[CreateAssetMenu(menuName = "Events/GhostEvent")]
public class GhostEvent: EventClass
{
    public override bool Entered(CarriageClass room)
    {
        Debug.Log("Ghost Entered");
        return true;
    }
    public override bool Exited(CarriageClass room)
    {
        Debug.Log("Ghost Exited");
        return true;
    }
    public override bool Triggered(CarriageClass room)
    {
        Debug.Log("Ghost Triggered");
        return true;
    }
    public override bool Generated(CarriageClass room) 
    {
        Debug.Log("Ghost Generated");
        return true; 
    }
    public override bool CallForDeletion(CarriageClass room) 
    {
        Debug.Log("Ghost Deleted");
        return true; 
    }
}
