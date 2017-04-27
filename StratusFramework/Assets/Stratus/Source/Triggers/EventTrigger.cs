/******************************************************************************/
/*!
@file   EventTrigger.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.Events;

namespace Stratus
{
  /// <summary>
  /// Triggers an event when the specified condition is met.
  /// </summary>
  public abstract class EventTrigger : StratusBehaviour
  {
    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    public class TriggerEvent : Stratus.Event {}
    public class EnableEvent : Stratus.Event { public bool Enabled;
      public EnableEvent(bool enabled) { Enabled = enabled; } }
    public enum DeliveryMethod { Event, Invoke }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    //[Tooltip("Whether we are currently debugging this trigger")]
    private bool IsDebug = false;

    [Header("Targeting")]
    [Tooltip("What component to send the trigger event to")]
    public EventDispatcher Target;
    [Tooltip("Whether the trigger will be sent to the GameObject as an event or invoked directly on the dispatcher component")]
    public DeliveryMethod Delivery = DeliveryMethod.Event;
    [Tooltip("Whether it should also trigger all of the object's children")]
    public bool Recursive = false;
    [Tooltip("Whether the trigger should persist after being triggered")]
    public bool Persistent = true;


    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    void Start()
    {
      this.OnInitialize();
      this.gameObject.Connect<EnableEvent>(this.OnEnableEvent);
    }

    void OnEnableEvent(EnableEvent e)
    {
      enabled = true;
      this.OnEnabled();
    }

    /// <summary>
    /// Triggers the event
    /// </summary>
    protected void Trigger()
    {
      if (enabled)
      {
        if (Delivery == DeliveryMethod.Event)
        {
          if (!this.Target)
          {
            Trace.Error("No target set!", this, true);
          }

          this.Target.gameObject.Dispatch<TriggerEvent>(new TriggerEvent());
          if (this.Recursive)
          {
            foreach(var child in this.Target.gameObject.Children())
            {
              child.Dispatch<TriggerEvent>(new TriggerEvent());
            }
          }
        }

        else if (Delivery == DeliveryMethod.Invoke)
        {
          this.Target.Trigger();
        }

        // If not persistent, disable
        if (!Persistent)
        {          
          this.enabled = false;
        }

        if (IsDebug) Trace.Script("Triggering!", this);
      }
    }
    
    /// <summary>
    /// Manually dispatches a trigger event onto the specified game bject.
    /// </summary>
    /// <param name="gameObject"></param>
    public static void Trigger(GameObject gameObject)
    {
      gameObject.Dispatch<TriggerEvent>(new TriggerEvent());
    }

    protected abstract void OnInitialize();
    protected abstract void OnEnabled();
    



  }

}