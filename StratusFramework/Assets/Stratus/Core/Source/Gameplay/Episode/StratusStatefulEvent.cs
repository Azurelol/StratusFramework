using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
  public class StratusStatefulEvent : StratusTriggerable
  {
    [Tooltip("The scope for this event")]
    public StratusEvent.Scope eventScope = StratusEvent.Scope.GameObject;

    [Tooltip("The stateful object")]
    [DrawIf("eventScope", StratusEvent.Scope.GameObject, ComparisonType.Equals)]
    public StratusStatefulObject target;

    [Tooltip("The event type")]
    public StratusStatefulObject.EventType eventType;
    [Tooltip("The label for the given state")]

    [DrawIf(nameof(usesLabel))]
    public string state;

    public bool usesLabel => eventType == StratusStatefulObject.EventType.Save || eventType == StratusStatefulObject.EventType.Load;

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
        case StratusEvent.Scope.GameObject:
          target.gameObject.Dispatch<StratusStatefulObject.StateEvent>(new StratusStatefulObject.StateEvent(eventType, state));
          break;

        case StratusEvent.Scope.Scene:
          Scene.Dispatch<StratusStatefulObject.StateEvent>(new StratusStatefulObject.StateEvent(eventType, state));
          break;
      }

    }
  }

}