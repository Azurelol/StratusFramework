/******************************************************************************/
/*!
@file   EventTrigger.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Stratus
{
  /// <summary>
  /// Triggers an event when the specified condition is met.
  /// </summary>
  public abstract class EventTrigger : StratusBehaviour
  {
    public class TriggerEvent : Stratus.Event {}
    public class EnableEvent : Stratus.Event { public bool Enabled;
      public EnableEvent(bool enabled) { Enabled = enabled; } }
    public enum TriggerScope { Event, Invoke }

    public bool Enabled = true;
    public EventDispatcher Target;
    [Tooltip("Whether the trigger will be sent to the GameObject as an event, or invoked directly on the dispatcher component")]
    public TriggerScope Scope = TriggerScope.Event;
    public bool IncludeChildren = false;
    bool Tracing = false;

    /**************************************************************************/
    /*!
    @brief  Initializes the EventTrigger.
    */
    /**************************************************************************/
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

    protected void Trigger()
    {
      if (Enabled)
      {
        if (Scope == TriggerScope.Event)
        {
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


        if (Tracing) Trace.Script("Triggering!", this);
      }
    }

    /**************************************************************************/
    /*!
    @brief Manually dispatches the trigger event onto the specified game bject.
    @param gameObject A reference to the gameobject in question.
    */
    /**************************************************************************/
    public static void Trigger(GameObject gameObject)
    {
      gameObject.Dispatch<TriggerEvent>(new TriggerEvent());
    }

    protected abstract void OnInitialize();
    protected abstract void OnEnabled();
    



  }

}