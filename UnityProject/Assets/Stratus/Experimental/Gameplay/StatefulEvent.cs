using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  public class StatefulEvent : Triggerable
  {
    public Event.Scope eventScope = Event.Scope.GameObject;
    [DrawIf("eventScope", Event.Scope.GameObject, ComparisonType.Equals)]
    public Stateful target;

    public Stateful.EventType eventType;
    [Tooltip("The label for the given state")]

    [DrawIf("eventType", Stateful.EventType.Load, ComparisonType.Equals)]
    public Stateful.LoadEventType load;

    [DrawIf(typeof(StatefulEvent), nameof(usesLabel))]
    public string state;

    public bool usesLabel => eventType == Stateful.EventType.Save ||
        (eventType == Stateful.EventType.Load 
        && load == Stateful.LoadEventType.Specific);

    protected override void OnAwake()
    {      
    }

    protected override void OnReset()
    {      
    }

    protected override void OnTrigger()
    {
      switch (eventScope)
      {
        case Event.Scope.GameObject:
          if (eventType == Stateful.EventType.Save)
            target.gameObject.Dispatch<Stateful.SaveEvent>(new Stateful.SaveEvent(state));
          else if (eventType == Stateful.EventType.Load)
            target.gameObject.Dispatch<Stateful.LoadEvent>(new Stateful.LoadEvent(state, load));
          break;

        case Event.Scope.Scene:
          if (eventType == Stateful.EventType.Save)
            Scene.Dispatch<Stateful.SaveEvent>(new Stateful.SaveEvent(state));
          else if (eventType == Stateful.EventType.Load)
            Scene.Dispatch<Stateful.LoadEvent>(new Stateful.LoadEvent(state, load));
          break;
      }

    }

    //public static bool UsesLabel(object statefulEvent)
    //{
    //  return statefulEvent.eventType == Stateful.EventType.Save || 
    //    (statefulEvent.eventType == Stateful.EventType.Load
    //    && statefulEvent.load == Stateful.LoadEventType.Specific);
    //}

    public bool UsesLabel()
    {
      return eventType == Stateful.EventType.Save ||
        (eventType == Stateful.EventType.Load 
        && load == Stateful.LoadEventType.Specific);
    }
  }

}