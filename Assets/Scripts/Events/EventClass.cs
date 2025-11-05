using System;
using UnityEngine;

// [CreateAssetMenu(menuName = "Event/Blank")]
[Serializable]
public class EventClass : ScriptableObject
{
    public virtual bool Entered(GameObject room) { return false; }
    public virtual bool Exited(GameObject room) { return false; }
    public virtual bool Triggered(GameObject room) { return false; }
}

[CreateAssetMenu(menuName = "Events/TestEvent")]
public class TestEvent : EventClass
{
    public override bool Entered(GameObject room)
    {
        Debug.Log("Test Entered");
        return true;
    }
    public override bool Exited(GameObject room)
    {
        Debug.Log("Test Exited");
        return true;
    }
    public override bool Triggered(GameObject room)
    {
        Debug.Log("Test Triggered");
        return true;
    }
}

[CreateAssetMenu(menuName = "Events/GhostEvent")]
public class GhostEvent: EventClass
{
    public override bool Entered(GameObject room)
    {
        Debug.Log("Ghost Entered");
        return true;
    }
    public override bool Exited(GameObject room)
    {
        Debug.Log("Ghost Exited");
        return true;
    }
    public override bool Triggered(GameObject room)
    {
        Debug.Log("Ghost Triggered");
        return true;
    }
}
