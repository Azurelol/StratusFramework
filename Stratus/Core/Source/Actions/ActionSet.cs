/******************************************************************************/
/*!
@file   ActionSet.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace Stratus
{  
  using ActionContainer = System.Collections.Generic.List<Action>;

  /// <summary>
  /// The ActionSet is the base class from which all other sets derive.
  /// Sets such as Sequence, Group and the unique set used by entities.
  /// </summary>
  public abstract class ActionSet : Action
  {
    public TimeScale Mode = TimeScale.Delta;
    protected ActionContainer ActiveActions = new ActionContainer();
    protected ActionContainer RecentlyAddedActions = new ActionContainer();
    public bool TraceStack = false;

    public ActionSet(string type, TimeScale mode) : base(type)
    {
      Mode = mode;
    }
    public override abstract float Update(float dt);

    /// <summary>
    /// Add an action to this set
    /// </summary>
    /// <param name="action">The specified action.</param>
    public virtual void Add(Action action)
    { 
      if (TraceStack)
      {
        StackTrace stackTrace = new StackTrace();
        var methodName = stackTrace.GetFrame(2).GetMethod().Name;
        //var className = stackTrace.GetFrame(2).GetMethod().ReflectedType.Name;
        Trace.Script("Adding " + action.Type + " from " + methodName);
      }

      if (Actions.Debugging) Trace.Script("'" + action.Type + "'");
      this.RecentlyAddedActions.Add(action);
    }
    
    /// <summary>
    /// Migrates new actions over.
    /// </summary>
    public void Migrate()
    {
      // Add the new actions (to prevent desync)
      foreach (var action in RecentlyAddedActions)
      {        
        ActiveActions.Add(action);
      }
      RecentlyAddedActions.Clear();
    }

    /// <summary>
    /// Sweeps all inactive actions.
    /// </summary>
    public void Sweep()
    {
      // No actions to clear
      if (this.ActiveActions.Count == 0)
        return;

      // Remove all actions that are finished
      this.ActiveActions.RemoveAll(x => x.isFinished == true);
    }

    /// <summary>
    /// Clears all actions.
    /// </summary>
    public void Clear()
    {
      this.ActiveActions.Clear();
      this.RecentlyAddedActions.Clear();
    }


  }

}

