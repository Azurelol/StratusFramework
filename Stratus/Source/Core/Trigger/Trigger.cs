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
  public abstract class Trigger : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    /// <summary>
    /// When received by a triggerable component, it will activate it
    /// </summary>
    public class TriggerEvent : Stratus.Event { }

    /// <summary>
    /// When received by a trigger, enables it
    /// </summary>
    public class EnableEvent : Stratus.Event
    {
      public bool enabled;
      public EnableEvent(bool enabled) { this.enabled = enabled; }
    }

    /// <summary>
    /// How the trigger is delivered to the target triggerable
    /// </summary>
    public enum DeliveryMethod
    {
      [Tooltip("All triggerables on the target's GameObject will be triggered")]
      All,
      [Tooltip("Only the target triggerable will be triggered")]
      Single
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Header("Targeting")]
    [Tooltip("What component to send the trigger event to")]
    public Stratus.Triggerable target;
    [Tooltip("Whether the trigger will be sent to the GameObject as an event or invoked directly on the dispatcher component")]
    public DeliveryMethod delivery = DeliveryMethod.All;
    [Tooltip("Whether it should also trigger all of the object's children")]
    public bool recursive = false;
    [Header("Lifetime")]
    [Tooltip("Whether the trigger should persist after being activated")]    
    public bool persistent = true;

    /// <summary>
    /// Subscribe to be notified when this trigger has been activated
    /// </summary>
    public UnityAction onActivate { get; set; } = () => {};

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether we are currently debugging the trigger (development purposes)
    /// </summary>
    private bool isDebug => false;

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// On awake, thee trigger first initializes the subclass before connecting to enabled events
    /// </summary>
    void Awake()
    {
      this.OnInitialize();
      this.gameObject.Connect<EnableEvent>(this.OnEnableEvent);
    }

    /// <summary>
    /// If the trigger was initially disabled,, enables it
    /// </summary>
    /// <param name="e"></param>
    void OnEnableEvent(EnableEvent e)
    {
      enabled = true;
      this.OnEnabled();
    }

    /// <summary>
    /// Triggers the event
    /// </summary>
    protected void Activate()
    {
      if (enabled)
      {
        if (delivery == DeliveryMethod.All)
        {
          if (!this.target)
          {
            Trace.Error("No target set!", this, true);
          }

          this.target.gameObject.Dispatch<TriggerEvent>(new TriggerEvent());
          if (this.recursive)
          {
            foreach (var child in this.target.gameObject.Children())
            {
              child.Dispatch<TriggerEvent>(new TriggerEvent());
            }
          }
        }

        else if (delivery == DeliveryMethod.Single)
        {
          this.target.Trigger();
        }

        // If not persistent, disable
        if (!persistent)
        {
          this.enabled = false;
        }

        this.onActivate();
        if (isDebug) Trace.Script("Triggering!", this);
      }
    }

    /// <summary>
    /// Manually dispatches a trigger event onto the specified game bject.
    /// </summary>
    /// <param name="gameObject"></param>
    public static void Activate(GameObject gameObject)
    {
      gameObject.Dispatch<TriggerEvent>(new TriggerEvent());
    }

    /// <summary>
    /// Invoked the first time this trigger is initialized
    /// </summary>
    protected abstract void OnInitialize();

    /// <summary>
    /// Invoked when this trigger has been enabled
    /// </summary>
    protected abstract void OnEnabled();




  }

}