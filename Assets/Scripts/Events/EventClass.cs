using System;
using UnityEngine;

// [CreateAssetMenu(menuName = "Event/Blank")]
[Serializable]
public class EventClass : ScriptableObject
{
    public virtual bool Enter() { return false; }
    public virtual bool Exit() { return false; }
    public virtual bool Trigger() { return false; }
}

[CreateAssetMenu(menuName = "Events/TestEvent")]
public class TestEvent: EventClass
{
    public override bool Enter()
    {
        Debug.Log("Entered");
        return true;
    }
}
