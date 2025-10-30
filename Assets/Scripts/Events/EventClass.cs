using System;
using UnityEngine;

// [CreateAssetMenu(menuName = "Event/Blank")]
[Serializable]
public class EventClass : ScriptableObject
{
    public virtual bool Entered() { return false; }
    public virtual bool Exited() { return false; }
    public virtual bool Triggered() { return false; }
}

[CreateAssetMenu(menuName = "Events/TestEvent")]
public class TestEvent : EventClass
{
    public override bool Entered()
    {
        Debug.Log("Test Entered");
        return true;
    }
    public override bool Exited()
    {
        Debug.Log("Test Exited");
        return true;
    }
    public override bool Triggered()
    {
        Debug.Log("Test Triggered");
        return true;
    }
}

[CreateAssetMenu(menuName = "Events/GhostEvent")]
public class GhostEvent: EventClass
{
    public override bool Entered()
    {
        Debug.Log("Ghost Entered");
        return true;
    }
    public override bool Exited()
    {
        Debug.Log("Ghost Exited");
        return true;
    }
    public override bool Triggered()
    {
        Debug.Log("Ghost Triggered");
        return true;
    }
}
