using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Triggers another event dispatcher
  /// </summary>
  public class TriggerEvent : EventDispatcher
  {
    [Header("Targeting")]
    [Tooltip("What component to send the trigger event to")]
    public EventDispatcher Target;
    [Tooltip("Whether the trigger will be sent to the GameObject as an event or invoked directly on the dispatcher component")]
    public EventTrigger.DeliveryMethod Delivery = EventTrigger.DeliveryMethod.Event;
    [Tooltip("Whether it should also trigger all of the object's children")]
    public bool Recursive = false;

    protected override void OnInitialize()
    {      
    }

    protected override void OnTrigger()
    {
      if (Delivery == EventTrigger.DeliveryMethod.Event)
      {
        if (!this.Target)
        {          
        }

        this.Target.gameObject.Dispatch<EventTrigger.TriggerEvent>(new EventTrigger.TriggerEvent());
        if (this.Recursive)
        {
          foreach (var child in this.Target.gameObject.Children())
          {
            child.Dispatch<EventTrigger.TriggerEvent>(new EventTrigger.TriggerEvent());
          }
        }
      }

      else if (Delivery == EventTrigger.DeliveryMethod.Invoke)
      {
        this.Target.Trigger();
      }
    }
  }

}