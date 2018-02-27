using UnityEngine;

namespace Stratus
{
  /// <summary>
  /// Base class for all trigger-related components in the Stratus Trigger framework
  /// </summary>
  public abstract class TriggerBase : StratusBehaviour
  {
    /// <summary>
    /// Whether this component has specific restart behaviour
    /// </summary>
    public interface IRestartable
    {
      void OnRestart();
    }

    //------------------------------------------------------------------------/
    // Events
    //------------------------------------------------------------------------/
    /// <summary>
    /// Signals the GameObject it's on that this basetrigger has been active
    /// </summary>
    public class ActivityEvent : Stratus.Event
    {
      public TriggerBase triggerBase;
    }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Tooltip("A short description of what this is for")]
    public string description;
    protected abstract void OnReset();
    //protected virtual void OnRestart() {}

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether this component has triggered
    /// </summary>
    public bool activated { get; protected set; }

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void Reset()
    {
      // If a trigger system is present, hide this component and set its default
      CheckForTriggerSystem();
      // Call subclass reset
      OnReset();
    }

    //------------------------------------------------------------------------/
    // Methods: Public
    //------------------------------------------------------------------------/
    /// <summary>
    /// Restarts this trigger to its initial state
    /// </summary>
    public void Restart()
    {
      activated = false;
      enabled = true;
      var restartable = this as IRestartable;
      restartable?.OnRestart();
      //OnRestart();
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void CheckForTriggerSystem()
    {
      var triggerSystem = gameObject.GetComponent<TriggerSystem>();
      if (triggerSystem)
      {
        this.hideFlags = HideFlags.HideInInspector;
        triggerSystem.Add(this);
      }
      else
      {
        this.hideFlags = HideFlags.None;
      }
    }




  }

}