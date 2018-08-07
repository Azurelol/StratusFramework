using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
  /// <summary>
  /// Triggers another event dispatcher
  /// </summary>
  public class TriggerAnotherEvent : Triggerable
  {
    [Header("Targeting")]
    [Tooltip("What component to send the trigger event to")]
    public Triggerable target;
    [Tooltip("Whether the trigger will be sent to the GameObject as an event or invoked directly on the dispatcher component")]
    public Trigger.Scope delivery = Trigger.Scope.GameObject;
    [Tooltip("Whether it should also trigger all of the object's children")]
    public bool recursive = false;

    protected override void OnAwake()
    {      
    }

    protected override void OnReset()
    {

    }

    protected override void OnTrigger()
    {
      if (this.delivery == Trigger.Scope.GameObject)
      {
        if (!this.target)
        {          
        }

        this.target.gameObject.Dispatch(new Trigger.TriggerEvent());
        if (this.recursive)
        {
          foreach (var child in this.target.gameObject.Children())
          {
            child.Dispatch(new Trigger.TriggerEvent());
          }
        }
      }

      else if (this.delivery == Trigger.Scope.Component)
      {
        this.target.Activate();
      }
    }
  }

}