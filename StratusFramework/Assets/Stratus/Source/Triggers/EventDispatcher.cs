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
  /// When triggered by an EventTrigger, will perform the specified action.
  /// </summary>
  public abstract class EventDispatcher : StratusBehaviour
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
    // Members
    //------------------------------------------------------------------------/
    bool HasInitialized = false;

    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/
    abstract protected void OnInitialize();
    abstract protected void OnTrigger();

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
    
    /// <summary>
    /// Called when the behavior is disabled or inactive.
    /// </summary>
    void OnDisable()
    {
      //Trace.Script("Hi", this);
      if (!HasInitialized)
      {
        this.Initialize();
      }
    }

    /// <summary>
    /// Initializes the EventDispatcher.
    /// </summary>
    void Initialize()
    {
      this.gameObject.Connect<EventTrigger.TriggerEvent>(this.OnTriggerEvent);
      this.OnInitialize();
      HasInitialized = true;
    }

    /// <summary>
    /// When the trigger event is received, runs the trigger sequence.
    /// </summary>
    /// <param name="e"></param>
    protected void OnTriggerEvent(EventTrigger.TriggerEvent e)
    {
      this.RunTriggerSequence();
    }

    /// <summary>
    /// Triggers the EventDispatcher.
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

    protected  void PostTrigger()
    {
    }

  }

}