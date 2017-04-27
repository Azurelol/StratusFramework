/******************************************************************************/
/*!
@file   Action.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;

namespace Stratus
{  
  /// <summary>
  /// Action is the base class from which all other actions derive from.
  /// </summary>
  public abstract class Action
  {
    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    static int Created = 0;
    static int Destroyed = 0;
    //--------------------------/
    /// <summary>
    /// The name of this action
    /// </summary>
    public string Type { get; protected set; }
    /// <summary>
    /// A private identifier for this action.
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// How much time has elapsed since the action started running
    /// </summary>
    public float Elapsed = 0.0f;
    /// <summary>
    /// The total amount of time the action will run for
    /// </summary>
    public float Duration = 0.0f;
    /// <summary>
    /// Whether the action is currently active. If not active it may end up
    /// blocking others behind it (if its on a sequence).
    /// </summary>
    public bool IsActive { get; private set; }
    /// <summary>
    /// Whether the action has finished running.
    /// </summary>
    public bool IsFinished = false;
    
    /// <summary>
    /// Whether we are logging actions
    /// </summary>
    static protected bool Tracing = false;
    //------------------------------------------------------------------------/
    // Interface
    //------------------------------------------------------------------------/
    public abstract float Update(float dt);

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    public Action(string type) 
    {
      this.Type = type;
      this.ID = Created++;
    }

    ~Action()
    {
      Destroyed++;
    }

    /// <summary>
    /// Resumes running the action.
    /// </summary>
    public void Resume()
    {
      this.IsActive = true;
    }

    /// <summary>
    /// Pauses the update of this action. This will block a sequence
    /// if there's other actions behind it.
    /// </summary>
    public void Pause()
    {
      this.IsActive = false;
    }

    /// <summary>
    /// Cancels execution of this action. It will cleaned up at the next opportunity.
    /// </summary>
    public void Cancel()
    {
      this.IsActive = false;
      this.IsFinished = true;
    }
    

  }


}

