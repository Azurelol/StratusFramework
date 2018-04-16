using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  public class StatefulEvent : Triggerable
  {
    [Tooltip("The scope for this event")]
    public Event.Scope eventScope = Event.Scope.GameObject;

    [Tooltip("The stateful object")]
    [DrawIf("eventScope", Event.Scope.GameObject, ComparisonType.Equals)]
    public Stateful target;

    [Tooltip("The event type")]
    public Stateful.EventType eventType;
    [Tooltip("The label for the given state")]

    [DrawIf(nameof(usesLabel))]
    public string state;

    public bool usesLabel => eventType == Stateful.EventType.Save || eventType == Stateful.EventType.Load;

    public override string automaticDescription
    {
      get
      {
        if (target)
        {
          string value = $"{eventType}";
          if (usesLabel)
            value += $" state '{state}'";
          value += $" on {target.name}";

          return value;
        }
        return string.Empty;
      }
    }

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
          target.gameObject.Dispatch<Stateful.StateEvent>(new Stateful.StateEvent(eventType, state));
          break;

        case Event.Scope.Scene:
          Scene.Dispatch<Stateful.StateEvent>(new Stateful.StateEvent(eventType, state));
          break;
      }

    }
  }

}