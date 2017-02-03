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
    public enum TriggerScope { Event, Invoke }

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    [Header("Targeting")]
    [Tooltip("Whether this trigger is enabled")]
    public bool Enabled = true;
    [Tooltip("What component to send the trigger event to")]
    public EventDispatcher Target;
    [Tooltip("Whether the trigger will be sent to the GameObject as an event, or invoked directly on the dispatcher component")]
    public TriggerScope Scope = TriggerScope.Event;
    public bool IncludeChildren = false;
    bool Tracing = false;
    [Tooltip("Whether the trigger should stay enabled after being triggered")]
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
      this.Enabled = true;
      this.OnEnabled();
    }

    /// <summary>
    /// Triggers the event
    /// </summary>
    protected void Trigger()
    {
      if (Enabled)
      {
        if (Scope == TriggerScope.Event)
        {
          if (!this.Target)
          {
            Trace.Error("No target set!", this, true);
          }

          this.Target.gameObject.Dispatch<TriggerEvent>(new TriggerEvent());
          if (this.IncludeChildren)
          {
            foreach(var child in this.Target.gameObject.Children())
            {
              child.Dispatch<TriggerEvent>(new TriggerEvent());
            }
          }
        }

        else if (Scope == TriggerScope.Invoke)
        {
          this.Target.Trigger();
        }

        // If not persistent, disable
        if (!Persistent)
          this.Enabled = false;

        if (Tracing) Trace.Script("Triggering!", this);
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