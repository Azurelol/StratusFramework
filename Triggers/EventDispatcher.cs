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
  public abstract class EventDispatcher : MonoBehaviour
  {    
    //------------------------------------------------------------------------/
    public bool Tracing = false;    

    [Tooltip("How long after activation before the event is fired")] public float Delay;

    //------------------------------------------------------------------------/
    abstract protected void OnInitialize();
    abstract protected void OnTrigger();
    //abstract protected void OnTrigger(GameObject obj);
    // Hack
    bool Initialized = false;
    
    /// <summary>
    /// Called when the script instance is first being loaded.
    /// </summary>
    void Awake()
    {
      //Trace.Script("Hey");
      this.Initialize();
    }
    
    /// <summary>
    /// Called when the behavior is disabled or inactive.
    /// </summary>
    void OnDisable()
    {
      //Trace.Script("Hi", this);
      if (!Initialized)
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
      //if (Tracing) Trace.Script("Initialized!", this);
      Initialized = true;
    }

    /// <summary>
    /// Triggers the EventDispatcher.
    /// </summary>
    public void Trigger()
    {
      this.RunTriggerSequence();
    }

    /// <summary>
    /// Triggers the EventDispatcher.
    /// </summary>
    /// <param name="obj">The object which triggered it</param>
    //public void Trigger(GameObject obj)
    //{
    //  this.RunTriggerSequence();
    //}

    /**************************************************************************/
    /*!
    @brief When the trigger event is received, runs the trigger sequence.
    */
    /**************************************************************************/
    protected void OnTriggerEvent(EventTrigger.TriggerEvent e)
    {      
      this.RunTriggerSequence();
    }

    /**************************************************************************/
    /*!
    @brief Runs the trigger sequence. After a specified delay, it will invoke
           the abstract 'OnTrigger' method.
    */
    /**************************************************************************/
    protected void RunTriggerSequence()
    {      
      //if (Tracing) Trace.Script("Delay = '" + this.Delay + "'", this);
      var seq = Actions.Sequence(this.gameObject.Actions());
      Actions.Delay(seq, this.Delay);
      Actions.Call(seq, this.OnTrigger);
      Actions.Call(seq, this.PostTrigger);
    }

    protected  void PostTrigger()
    {
      //if (!Persistent)
      //{
      //  Destroy(this.gameObject);
      //}
    }

  }

}