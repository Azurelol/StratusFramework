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


namespace Stratus
{
  /// <summary>
  /// A component that when triggered will perform a specific action.
  /// </summary>
  public abstract class Triggerable : MonoBehaviour
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Whether we are printing debug output
    /// </summary>
    [Tooltip("Whether we are printing debug output")]
    public bool Tracing = false;

    /// <summary>
    /// Whether this event dispatcher will respond to trigger events
    /// </summary>
    [Tooltip("How long after activation before the event is fired")]
    public float Delay;
    
    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/
    abstract protected void OnAwake();
    abstract protected void OnTrigger();
    virtual protected void PreInitialize() {}

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    /// <summary>
    /// Called when the script instance is first being loaded.
    /// </summary>
    void Awake()
    {
      this.Initialize();
    }
    
    ///// <summary>
    ///// Called when the behavior is disabled or inactive.
    ///// </summary>
    //void OnDisable()
    //{
    //  //Trace.Script("Hi", this);
    //  if (!initialized)
    //  {
    //    this.Initialize();
    //  }
    //}

    /// <summary>
    /// Initializes the EventDispatcher.
    /// </summary>
    protected virtual void Initialize()
    {
      this.gameObject.Connect<Trigger.TriggerEvent>(this.OnTriggerEvent);
      this.PreInitialize();
      this.OnAwake();
    }

    /// <summary>
    /// When the trigger event is received, runs the trigger sequence.
    /// </summary>
    /// <param name="e"></param>
    protected void OnTriggerEvent(Trigger.TriggerEvent e)
    {
      this.RunTriggerSequence();
    }

    /// <summary>
    /// Activated the trigger sequence if enabled
    /// </summary>
    public void Trigger()
    {
      if (!enabled)
        return;
      this.RunTriggerSequence();
    }
    

    /// <summary>
    /// Runs the trigger sequence. After a specified delay, it will invoke
    /// the abstract 'OnTrigger' method.
    /// </summary>
    protected void RunTriggerSequence()
    {
      //if (Tracing) Trace.Script("Delay = '" + this.Delay + "'", this);
      if (Tracing) Trace.Script("Triggering", this);
      var seq = Actions.Sequence(this.gameObject.Actions());
      Actions.Delay(seq, this.Delay);
      Actions.Call(seq, this.OnTrigger);
      Actions.Call(seq, this.PostTrigger);
    }

    protected virtual void PostTrigger() {}



  }

}