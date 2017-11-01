using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Triggers another event dispatcher
  /// </summary>
  public class TriggerAnotherEvent : Triggerable
  {
    [Header("Targeting")]
    [Tooltip("What component to send the trigger event to")]
    public Triggerable Target;
    [Tooltip("Whether the trigger will be sent to the GameObject as an event or invoked directly on the dispatcher component")]
    public Trigger.DeliveryMethod Delivery = Stratus.Trigger.DeliveryMethod.All;
    [Tooltip("Whether it should also trigger all of the object's children")]
    public bool Recursive = false;

    protected override void OnAwake()
    {      
    }

    protected override void OnTrigger()
    {
      if (this.Delivery == Stratus.Trigger.DeliveryMethod.All)
      {
        if (!this.Target)
        {          
        }

        this.Target.gameObject.Dispatch(new Trigger.TriggerEvent());
        if (this.Recursive)
        {
          foreach (var child in this.Target.gameObject.Children())
          {
            child.Dispatch(new Trigger.TriggerEvent());
          }
        }
      }

      else if (this.Delivery == Stratus.Trigger.DeliveryMethod.Single)
      {
        this.Target.Trigger();
      }
    }
  }

}