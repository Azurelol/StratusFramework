/******************************************************************************/
/*!
@file   EventDispatcher.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using UnityEngine.Events;

namespace Stratus
{
  /// <summary>
  /// A component that when triggered will perform a specific action.
  /// </summary>
  public abstract class Triggerable : TriggerBase
  {
    /// <summary>
    /// This event signals that the triggerable has finished
    /// </summary>
    public class EndedEvent : Stratus.Event {}

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether we are printing debug output
    /// </summary>
    [Tooltip("Whether we are printing debug output")]
    public bool logging = false;

    /// <summary>
    /// Whether this event dispatcher will respond to trigger events
    /// </summary>
    [Tooltip("How long after activation before the event is fired")]
    public float delay;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/  
    /// <summary>
    /// The latest received trigger event
    /// </summary>
    protected Trigger.TriggerEvent triggerEvent { get; private set; }

    /// <summary>
    /// The latest received instruction
    /// </summary>
    protected Trigger.Instruction instruction { get; private set; }

    /// <summary>
    /// Subscribe to be notified when this trigger has been activated
    /// </summary>
    public UnityAction<Triggerable> onTriggered { get; set; }

    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/
    abstract protected void OnAwake();
    abstract protected void OnTrigger();
    //protected virtual void OnTrigger(Trigger.Instruction instruction) {}
    virtual protected void PreAwake() {}

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Called when the script instance is first being loaded.
    /// </summary>
    void Awake()
    {
      this.gameObject.Connect<Trigger.TriggerEvent>(this.OnTriggerEvent);
      this.PreAwake();
      this.OnAwake();
      onTriggered = (Triggerable trigger) => {};
    }

    /// <summary>
    /// When the trigger event is received, runs the trigger sequence.
    /// </summary>
    /// <param name="e"></param>
    protected void OnTriggerEvent(Trigger.TriggerEvent e)
    {
      triggerEvent = e;
      instruction = triggerEvent.instruction;
      this.RunTriggerSequence();
    }

    /// <summary>
    /// Activates this triggerable
    /// </summary>
    public void Trigger()
    {
      if (!enabled)
        return;      
      this.RunTriggerSequence();
      activated = true;
    }    

    /// <summary>
    /// Runs the trigger sequence. After a specified delay, it will invoke
    /// the abstract 'OnTrigger' method.
    /// </summary>
    protected void RunTriggerSequence()
    {
      var seq = Actions.Sequence(this.gameObject.Actions());
      Actions.Delay(seq, this.delay);
      if (logging)
        Trace.Script($"Triggered {GetType().Name} - {description}", this);
      Actions.Call(seq, this.OnTrigger);
      Actions.Call(seq, ()=>onTriggered(this));      
    }

  }

}